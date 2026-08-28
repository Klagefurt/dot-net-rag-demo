using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
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

            // 3. Chunking options
            var chunkingSection = configuration.GetSection("Chunking");
            services.AddOptions<ChunkingOptions>()
                .Bind(chunkingSection)
                .ValidateDataAnnotations()
                .ValidateOnStart();

            // 4. Add chunker service
            services.AddSingleton<GermanTextChunker>();

            // 5. Add Rag Service
            services.AddScoped<RagService>();

            // 6. Add document ingestion service
            services.AddScoped<DocumentIngestionService>();

            return services;
        }
    }
}