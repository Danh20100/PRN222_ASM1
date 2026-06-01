using ChatBotRAG.DataAccess.Models;

namespace ChatBotRAG.DataAccess.Repositories
{
    public interface IDocumentRepository : IRepository<Document>
    {
    }

    public class DocumentRepository : Repository<Document>, IDocumentRepository
    {
        public DocumentRepository(Data.ChatBotDbContext context) : base(context)
        {
        }
    }
}
