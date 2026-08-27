namespace Rag.Core.Configuration
{
    public class ChunkingOptions
    {
        public int ChunkSize { get; set; } = 1200;
        public int Overlap { get; set; } = 200;
    }
}