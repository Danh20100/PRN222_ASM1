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
                // 2. Parse text
                string fullText = "";
                if (extension == ".pdf")
                {
                    fullText = ExtractTextFromPdf(filePath);
                }
                else if (extension == ".docx")
                {
                    fullText = ExtractTextFromDocx(filePath);
                }
                else if (extension == ".txt")
                {
                    fullText = await File.ReadAllTextAsync(filePath);
                }
                else
                {
                    document.Status = "failed";
                    await _documentRepository.UpdateAsync(document);
                    return false;
                }

                // 3. Chunking (Mock: split by words, ~500 words per chunk)
                var chunks = CreateChunks(fullText, 500);

                // 4. Save Chunks
                for (int i = 0; i < chunks.Count; i++)
                {
                    var chunk = new DocumentChunk
                    {
                        DocumentId = document.Id,
                        ChunkIndex = i,
                        Content = chunks[i],
                        PageNumber = null, // simplified
                        VectorDbId = "{ \"mock_vector\": [0.1, 0.2, 0.3] }" // Mock embedding
                    };
                    await _chunkRepository.AddAsync(chunk);
                }

                // 5. Mark Completed
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

        private string ExtractTextFromPdf(string filePath)
        {
            var sb = new StringBuilder();
            using (PdfDocument document = PdfDocument.Open(filePath))
            {
                foreach (var page in document.GetPages())
                {
                    var text = ContentOrderTextExtractor.GetText(page);
                    sb.AppendLine(text);
                }
            }
            return sb.ToString();
        }

        private string ExtractTextFromDocx(string filePath)
        {
            var sb = new StringBuilder();
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(filePath, false))
            {
                var body = wordDoc.MainDocumentPart?.Document.Body;
                if (body != null)
                {
                    foreach (var para in body.Elements<Paragraph>())
                    {
                        sb.AppendLine(para.InnerText);
                    }
                }
            }
            return sb.ToString();
        }

        private List<string> CreateChunks(string text, int wordsPerChunk)
        {
            var words = text.Split(new[] { ' ', '\r', '\n', '\t' }, StringSplitOptions.RemoveEmptyEntries);
            var chunks = new List<string>();
            for (int i = 0; i < words.Length; i += wordsPerChunk)
            {
                var chunkWords = words.Skip(i).Take(wordsPerChunk);
                chunks.Add(string.Join(" ", chunkWords));
            }
            return chunks;
        }
    }
}
