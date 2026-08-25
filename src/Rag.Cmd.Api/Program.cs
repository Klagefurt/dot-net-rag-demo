using Polly;
using Rag.Core;
using Rag.Core.Services;

var builder = WebApplication.CreateBuilder(args);

// Set base path
builder.Configuration.SetBasePath(Directory.GetCurrentDirectory());

builder.Configuration.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);

// Add environmental variables
//builder.Configuration.AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
//builder.Configuration.AddEnvironmentVariables();

builder.Services.AddInfrastructure(builder.Configuration);

builder.Services.AddHttpClient<OllamaEmbeddingService>((sp, client) =>
{
  var config = sp.GetRequiredService<IConfiguration>();

  string baseUrl = config["Ollama:BaseUrl"] ?? 
    throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");

  client.BaseAddress = new Uri(baseUrl);
})
.AddTransientHttpErrorPolicy(p => p.WaitAndRetryAsync(3, _ => TimeSpan.FromSeconds(2)));

builder.Services.AddHttpClient<OllamaLlmService>((sp, client) =>
{
  var config = sp.GetRequiredService<IConfiguration>();

  string baseUrl = config["Ollama:BaseUrl"] ?? 
    throw new InvalidOperationException("BaseUrl is not configured in appsettings.json");

  client.BaseAddress = new Uri(baseUrl);
});

builder.Services.AddControllers();

// Add swagger only for development stage
if (builder.Environment.IsDevelopment())
{
  builder.Services.AddEndpointsApiExplorer();
  builder.Services.AddSwaggerGen();
}

var app = builder.Build();

// MIDDLEWARE

if (app.Environment.IsDevelopment())
{
  app.UseSwagger();
  app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();