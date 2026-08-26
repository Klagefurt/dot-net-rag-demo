using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Rag.Core.Configuration;

namespace Rag.Core.Services
{
    public interface IOllamaEmbeddingService
    {
        Task<float[]> GetEmbeddingAsync(string text);
    }

    internal class OllamaEmbeddingService(HttpClient httpClient, IOptions<OllamaOptions> options) : IOllamaEmbeddingService
    {
        private readonly OllamaOptions _options = options.Value;

        public async Task<float[]> GetEmbeddingAsync(string text)
        {
            var request = new { model = _options.EmbeddingModel, prompt = text };
            
            var response = await httpClient.PostAsJsonAsync("api/embeddings", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaEmbedResponse>();
            return result?.Embedding ?? 
                throw new InvalidOperationException("Ollama returned an empty embedding");
        }
    }

    internal record OllamaEmbedResponse([property: JsonPropertyName("embedding")] float[] Embedding);
}