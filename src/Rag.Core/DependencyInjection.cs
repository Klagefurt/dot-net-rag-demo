using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Rag.Core.Configuration;
using Rag.Core.Data;
using Rag.Core.Services;

namespace Rag.Core
{
    public static class DependecyInjection
    {
        public static IServiceCollection AddInfrastructure(
            this IServiceCollection services, 
            IConfiguration configuration)
        {
            // 1. Ollama options
            var ollamaSection = configuration.GetSection("Ollama");
            services.AddOptions<OllamaOptions>()
                .Bind(ollamaSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // 2. Db Context
            string connectionString = configuration.GetConnectionString("DefaultConnection")
                ?? throw new InvalidOperationException("DefaultConnection is missing");

            services.AddDbContext<AppDbContext>(opts => 
                opts.UseNpgsql(connectionString, o => o.UseVector()));

            var hostSection = configuration.GetSection("HostSettings");
            services.AddOptions<HostSettings>()
                .Bind(hostSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // 3. Ollama http clients
            // Not best solution, but not to install here Microsoft.Extemsions.Http package
            // and to avoid version conflict under .Net 8.0
            services.AddTransient<OllamaEmbeddingService>();
            services.AddTransient<OllamaLlmService>();

            return services;
        }
    }
}