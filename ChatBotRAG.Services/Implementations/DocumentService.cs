using ChatBotRAG.DataAccess.Models;
using ChatBotRAG.DataAccess.Repositories;
using ChatBotRAG.Services.Interfaces;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UglyToad.PdfPig;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;

namespace ChatBotRAG.Services.Implementations
{
    public class DocumentService : IDocumentService
    {
        private readonly IDocumentRepository _documentRepository;
        private readonly IRepository<DocumentChunk> _chunkRepository;

        public DocumentService(IDocumentRepository documentRepository, IRepository<DocumentChunk> chunkRepository)
        {
            _documentRepository = documentRepository;
            _chunkRepository = chunkRepository;
        }

        public async Task<bool> ProcessFileAsync(string filePath, Guid sid)
        {
            if (!File.Exists(filePath))
                return false;

            var fileName = Path.GetFileName(filePath);
            var extension = Path.GetExtension(filePath).ToLower();

            // 1. Create Document Record
            var document = new DataAccess.Models.Document
            {
                SubjectId = sid,
                Title = fileName,
                FileType = extension,
                FileSize = new FileInfo(filePath).Length,
                FileUrl = filePath,
                Status = "processing",
                CreatedAt = DateTime.UtcNow
            };

            await _documentRepository.AddAsync(document);

            try
            {
                var startInfo = new System.Diagnostics.ProcessStartInfo
                {
                    FileName = "python",
                    Arguments = $"\"c:\\Users\\Admin\\OneDrive\\Desktop\\PRN222_ASM1\\PythonAI\\ingest.py\" \"{filePath}\"",
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false,
                    CreateNoWindow = true,
                    StandardOutputEncoding = Encoding.UTF8
                };

                using var process = System.Diagnostics.Process.Start(startInfo);
                if (process == null) throw new Exception("Failed to start python process");

                string output = await process.StandardOutput.ReadToEndAsync();
                await process.WaitForExitAsync();

                if (process.ExitCode != 0 || output.Contains("\"error\""))
                {
                    document.Status = "failed";
                    await _documentRepository.UpdateAsync(document);
                    return false;
                }

                var options = new System.Text.Json.JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var chunks = System.Text.Json.JsonSerializer.Deserialize<List<PythonChunkResult>>(output, options);

                if (chunks != null)
                {
                    for (int i = 0; i < chunks.Count; i++)
                    {
                        var chunk = new DocumentChunk
                        {
                            DocumentId = document.Id,
                            ChunkIndex = i,
                            Content = chunks[i].Content,
                            PageNumber = null,
                            VectorDbId = System.Text.Json.JsonSerializer.Serialize(chunks[i].Vector) 
                        };
                        await _chunkRepository.AddAsync(chunk);
                    }
                }

                document.Status = "completed";
                await _documentRepository.UpdateAsync(document);

                return true;
            }
            catch (Exception)
            {
                document.Status = "failed";
                await _documentRepository.UpdateAsync(document);
                return false;
            }
        }
    }

    public class PythonChunkResult
    {
        [System.Text.Json.Serialization.JsonPropertyName("content")]
        public string Content { get; set; } = string.Empty;

        [System.Text.Json.Serialization.JsonPropertyName("vector")]
        public List<float> Vector { get; set; } = new List<float>();
    }
}
