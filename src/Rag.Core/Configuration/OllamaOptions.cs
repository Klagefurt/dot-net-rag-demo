using System.ComponentModel.DataAnnotations;

namespace Rag.Core.Configuration
{
    internal class OllamaOptions
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "BaserUrl is required")]
        public string BaseUrl {get; set;} = "http://localhost:11434";
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "EmbeddingModel is required")]
        public string EmbeddingModel {get; set;} = "nomic-embed-text";
        
        [Required(AllowEmptyStrings = false, ErrorMessage = "LlmModel is required")]
        public string LlmModel { get; set; } = "llama3.2";
    }
}