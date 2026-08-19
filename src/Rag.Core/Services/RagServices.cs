using Rag.Core.Data;
using Pgvector;
using System.Data.Common;
using Pgvector.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Rag.Core.Services
{
    internal class RagServices(AppDbContext dbContext, 
        OllamaEmbeddingService embeddingService,
        OllamaLlmService llmService)
    {
        public async Task<RagResult> QueryAsync(string question, int topK = 5)
        {
            var rawVector = await embeddingService.GetEmbeddingAsync(question);
            var questionVector = new Vector(rawVector);

            // Vector search in a db
            var matchingChunks = await dbContext.Chunks
                .Select(c => new
                {
                    Chunk = c,
                    Score = 1 - c.Embedding!.CosineDistance(questionVector)
                })
                .OrderByDescending(x => x.Score)
                .Take(topK)
                .ToListAsync();

            var contextText = string.Join("\n\n", 
                matchingChunks.Select(c => c.Chunk.Content));

            string prompt = $"""
                Beantworten Sie die Frage nur auf Basis des bereitgestellten Kontextes. 
                Wenn Sie die Antwort im Kontext nicht finden, sagen Sie "Ich weiß es nicht".
                Antworten Sie immer auf Deutsch.

                KONTEXT:
                {contextText}

                FRAGE:
                {question}

                ANTWORT:
                """;

            var answer = await llmService.GetAnswerAsync(prompt);

            return new RagResult(
                Answer: answer,
                Sources: matchingChunks.Select(x => new SourceDto(
                    x.Chunk.Id,
                    x.Chunk.DocumentId,
                    x.Chunk.Content,
                    x.Chunk.ChunkIndex,
                    x.Score
                )).ToList()
            );
        }
    }

    internal record RagResult(string Answer, List<SourceDto> Sources);
    internal record SourceDto(long Id, long DocumentId, string Content, int ChunkIndex, double Score);
}