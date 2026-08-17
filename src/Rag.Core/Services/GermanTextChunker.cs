namespace Rag.Core.Services
{
    internal class GermanTextChunker(int chunkSize = 1000, int overlap = 150)
    {
        public List<string> SplitText(string text)
        {
            var result = new List<string>();
            if (chunkSize <= overlap) 
                throw new ArgumentException("Chunksize should be more than overlap!");

            int start = 0;
            int textLength = text.Length;

            while (start <= textLength)
            {
                var length = Math.Min(chunkSize, textLength - start);
                result.Add(text.Substring(start, length).Trim());
                start += chunkSize - overlap;
            }

            return result;
        }       
    }
}