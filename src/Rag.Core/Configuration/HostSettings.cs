using System.ComponentModel.DataAnnotations;

namespace Rag.Core.Configuration
{
    internal class HostSettings
    {
        [Required(AllowEmptyStrings = false, ErrorMessage = "BaserUrl is required")]
        public string BaseUrl {get; set;} = "http://localhost:11434";
    }
}