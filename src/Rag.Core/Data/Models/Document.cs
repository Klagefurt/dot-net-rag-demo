using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Rag.Core.Data.Models
{
    [Table("documents")]
    internal class Document
    {
        [Column("id")]
        public long Id { get; set; }

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("source_path")]
        public string SourcePath { get; set; }  = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public List<Chunk> Chunks { get; set; } = [];
    }
}