using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Rag.Core.Configuration;
using Rag.Core.Data;
using Rag.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Connect to db 
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ??
  throw new InvalidOperationException("Connection string 'DeafultConenction' not found");

builder.Services.AddDbContext<AppDbContext>(options => 
  options.UseNpgsql(connectionString, o => o.UseVector()));

// 2. Connect to appsettings.json and switch on onstart validation
var ollamaSection = builder.Configuration.GetSection("Ollama");
builder.Services.AddOptions<OllamaOptions>()
  .Bind(ollamaSection)
  .ValidateDataAnnotations()
  .ValidateOnStart();

string baseUrl = ollamaSection["BaseURL"]
  ?? throw new InvalidOperationException("Ollama: BaseUrl is missing in appsettings.json!");

// 3. Register Typed HttpClients
builder.Services.AddHttpClient<OllamaEmbeddingService>(client => client.BaseAddress = new Uri(baseUrl));
builder.Services.AddHttpClient<OllamaLlmService>(client => client.BaseAddress = new Uri(baseUrl));

builder.Services.AddScoped<RagService>();

var app = builder.Build();

// 4. Health check postgreSQL
app.MapGet("/health", async ([FromServices]AppDbContext db) => 
  await db.Database.CanConnectAsync()
    ? Results.Ok(new {status = "healthy"})
    : Results.StatusCode(500) 
  );

// 5. Main api endpoint for requests to RAG
app.MapPost("/api/query", async ([FromBody]QueryRequest request, [FromServices]RagService ragService) =>
{
  if (string.IsNullOrWhiteSpace(request.Question))
    return Results.BadRequest(new { error = "Question can not be empty!"} );
  
  var result = await ragService.QueryAsync(request.Question);
  return Results.Ok(result);
});

app.Run("http://localhost:8000");

internal record QueryRequest(string Question);