using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Pgvector;

namespace Rag.Core.Data.Models
{
    [Table("chunks")]
    internal class Chunk
    {
        [Key]
        [Column("id")]
        public long Id { get; set; }

        [Column("documentId")]
        public long DocumentId { get; set; }

        [ForeignKey(nameof(DocumentId))]
        public Document? Document { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("chunk_index")]
        public int ChunkIndex { get; set; }

        [Column("embedding", TypeName = "vector(768)")]
        public Vector? Embedding { get; set; }
        
        [Column("created_at")]
        public DateTime CreatedAt {get; set;} = DateTime.UtcNow;
    }
}