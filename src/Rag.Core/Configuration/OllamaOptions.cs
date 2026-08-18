using System.ComponentModel.DataAnnotations;

namespace Rag.Core.Configuration
{
    internal class OllamaOptions
    {
        [Required(AllowEmptyStrings = false)]
        public required string BaseUrl { get; set; }
        [Required(AllowEmptyStrings = false)]
        public required string EmbeddingModel {get; set;}
        [Required(AllowEmptyStrings = false)]
        public required string LlmModel { get; set; }
    }
}