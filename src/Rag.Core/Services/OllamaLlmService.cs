using System.Net.Http.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using Rag.Core.Configuration;

namespace Rag.Core.Services
{
    public interface IOllamaLlmService
    {
        Task<string> GetAnswerAsync(string prompt);
    }

    internal class OllamaLlmService(HttpClient httpClient, IOptions<OllamaOptions> options) : IOllamaLlmService
    {
        private readonly OllamaOptions _options = options.Value;

        public async Task<string> GetAnswerAsync(string prompt)
        {
            var request = new { model = _options.LlmModel, prompt, stream = false };

            var response = await httpClient.PostAsJsonAsync("api/generate", request);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<OllamaGenerateResponse>();
            return result?.Response ?? 
                throw new InvalidOperationException("Ollama returned an empty response");
        }
    }

    internal record OllamaGenerateResponse([property: JsonPropertyName("response")] string Response);
}