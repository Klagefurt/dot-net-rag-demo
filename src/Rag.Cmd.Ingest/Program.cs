using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rag.Core;
using Rag.Core.Services;

var configuration = new ConfigurationBuilder()
  .SetBasePath(AppContext.BaseDirectory)
  .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
  .Build();

var services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

services.AddInfrastructure(configuration);

string baseUrl = configuration["Ollama:baseUrl"] 
  ?? throw new InvalidOperationException("BaseUrl must be filled!");

services.AddHttpClient<IOllamaEmbeddingService, OllamaEmbeddingService>((client) =>
{
  client.BaseAddress = new Uri(baseUrl);
});

var serviceProvider = services.BuildServiceProvider();

using var scope = serviceProvider.CreateScope();

var ingestionService = scope.ServiceProvider.GetRequiredService<DocumentIngestionService>();

string docsDir = args.Length > 0
  ? args[0]
  : Path.Combine(AppContext.BaseDirectory, "../../../examples");

await ingestionService.IngestDirectoryAsync(docsDir);

Console.WriteLine("\n[FINISH] Der Indexierungsprozess ist beendet.");