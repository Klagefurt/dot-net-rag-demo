using Rag.Core.Configuration;
using Rag.Core.Data;

var builder = WebApplication.CreateBuilder(args);

// 1. Connect to appsettings.json and switch on strong onstart validation
builder.Services.AddOptions<OllamaOptions>()
  .Bind(builder.Configuration.GetSection("Ollama"))
  .ValidateDataAnnotations()
  .ValidateOnStart();

// 2. Connect to db 
builder.Services.AddDbContext<AppDbContext>();


var app = builder.Build();

app.MapGet("/", () => "Hello World!");

app.Run();
