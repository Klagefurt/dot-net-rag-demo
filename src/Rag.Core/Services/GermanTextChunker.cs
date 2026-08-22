using Microsoft.Extensions.Options;
using Rag.Core.Configuration;

namespace Rag.Core.Services
{
    public class GermanTextChunker(IOptions<ChunkingOptions> options)
    {
        private readonly ChunkingOptions _options = options.Value;

        public IEnumerable<string> Chunk(string text)
        {
            var result = new List<string>();
            if (_options.ChunkSize <= _options.Overlap) 
                throw new ArgumentException("Chunksize should be more than overlap!");

            int start = 0;
            int textLength = text.Length;

            while (start <= textLength)
            {
                var length = Math.Min(_options.ChunkSize, textLength - start);
                result.Add(text.Substring(start, length).Trim());
                start += _options.ChunkSize - _options.Overlap;
            }

            return result;
        }       
    }
}