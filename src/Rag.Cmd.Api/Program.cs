using Polly;
using Rag.Core;
using Rag.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Set base path and read configuration from appsettings.json
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add base infrastructure (db, chunker, services)
builder.Services.AddInfrastructure(builder.Configuration);

// Configure http clients on transport level
builder.Services.AddHttpClient<IOllamaEmbeddingService, OllamaEmbeddingService>((sp, client) =>
{
  var config = sp.GetRequiredService<IConfiguration>();

  string baseUrl = config["Ollama:BaseUrl"] ?? 
    throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");

  client.BaseAddress = new Uri(baseUrl);
})
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

builder.Services.AddHttpClient<IOllamaLlmService, OllamaLlmService>((sp, client) =>
{
  var config = sp.GetRequiredService<IConfiguration>();

  string baseUrl = config["Ollama:BaseUrl"] ?? 
    throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");

  client.BaseAddress = new Uri(baseUrl);
});

// Configure health check
string connectionString = builder.Configuration.GetConnectionString("DefaultConnection") 
  ?? throw new InvalidOperationException("No DefaultConnection!");
builder.Services.AddHealthChecks().AddNpgSql(connectionString);

// Define swagger only for development stage
if (builder.Environment.IsDevelopment())
{
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// MIDDLEWARE layer
if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// Add health check
app.MapHealthChecks("/health");

// Minimal API section
// TODO: implement!

app.Run("http://localhost:8080");