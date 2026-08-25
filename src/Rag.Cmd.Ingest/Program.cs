
using Microsoft.Extensions.Hosting;
using Rag.Core;

var host = Host.CreateDefaultBuilder(args)
  .ConfigureServices((context, services) =>
  {
    services.AddInfrastructure(context.Configuration);
  });
