using Microsoft.Extensions.Logging;
using Rag.Core.Data;
using Rag.Core.Data.Models;

namespace Rag.Core.Services
{
    internal class DocumentIngestionService(
        AppDbContext dbContext, 
        GermanTextChunker chunker, 
        OllamaEmbeddingService embeddingService, 
        ILogger<DocumentIngestionService> logger)
    {
        public async Task IngestDirectoryAsync(string docsDir)
        {
            if (!Directory.Exists(docsDir))
            {
                logger.LogWarning("Verzeichnis {DocsDir} nicht gefunden. Erstelle Demo-Ordner...", docsDir);
                Directory.CreateDirectory(docsDir);

                string demoFile = Path.Combine(docsDir, "firma_info.txt");
                string demoText = "Die 'Yolki-Metolki Ltd' ist ein führender deutscher Hersteller von Premium-Weihnachtsdekorationen. " +
                                  "Das Unternehmen wurde 2018 in der Stadt Sosnowka gegründet.\n\n" +
                                  "Das Sicherheitsüberprüfungssystem aktiviert sich automatisch, wenn die Eingangstür verriegelt wird. " +
                                  "Im Falle eines Einbruchs ertönt eine Sirene und eine Push-Benachrichtigung wird an Ihr Smartphone gesendet.";
                await File.WriteAllTextAsync(demoFile, demoText);
            }

            logger.LogInformation("Starte Indexierung im Verzeichnis: {DocsDir}", docsDir);
            var txtFiles = Directory.GetFiles(docsDir, "*.txt");

            if (txtFiles.Length == 0)
            {
                logger.LogWarning("Keine txt-Dateien im Ordner gefunden");
                return;
            }

            foreach (var filePath in txtFiles)
            {
                string title = Path.GetFileNameWithoutExtension(filePath);
                string fullText = await File.ReadAllTextAsync(filePath);

                logger.LogInformation("Verarbeite Datei: {Title}. Text wird in chunks zerlegt.", title);

                // 1. Save document metadata
                var doc = new Document{ Title = title, SourcePath = filePath };
                dbContext.Documents.Add(doc);
                await dbContext.SaveChangesAsync();

                // 2. Chunk text to chunks
                var textChunks = chunker.Chunk(fullText);
                logger.LogInformation("Datei '{Title}' erfolgreich in {Count} chunks unterteilt.", title, textChunks.Count);

                for (int i = 0; i < textChunks.Count; i++)
                {
                    string chunkContent = textChunks[i];
                    logger.LogInformation("Generiere Embedding for Chunk N{i}", i);

                    // 3. Get a 768-dimensional vector from nomic-embed-text model
                    float[]? rawVector = await embeddingService.GetEmbeddingAsync(chunkContent);

                    // 4. Save chunk to db
                    var chunk = new Chunk
                    {
                        DocumentId = doc.Id,
                        Content = chunkContent,
                        ChunkIndex = i,
                        Embedding = new Pgvector.Vector(rawVector)
                    };

                    dbContext.Chunks.Add(chunk);
                }

                await dbContext.SaveChangesAsync();
                logger.LogInformation("[ERFOLG] Dokument '{Title}' wurde erfolgreich in PostgreSQL gespeichert!", title);
            }
        }
    }
}