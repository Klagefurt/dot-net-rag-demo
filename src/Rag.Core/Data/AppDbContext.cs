using Microsoft.EntityFrameworkCore;
using Rag.Core.Data.Models;

namespace Rag.Core.Data
{
    internal class AppDbContext : DbContext
    {
        internal DbSet<Chunk> Chunks => Set<Chunk>();
        internal DbSet<Document> Documents =>  Set<Document>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.HasPostgresExtension("vector");

            modelBuilder.Entity<Chunk>()
                .HasOne(c => c.Document)
                .WithMany(d => d.Chunks)
                .HasForeignKey(c => c.DocumentId)
                .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}