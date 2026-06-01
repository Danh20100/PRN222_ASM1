using System.Threading.Tasks;

namespace ChatBotRAG.Services.Interfaces
{
    public interface IDocumentService
    {
        Task<bool> ProcessFileAsync(string filePath, Guid SubjectId);
    }
}
