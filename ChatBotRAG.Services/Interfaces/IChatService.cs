using System.Threading.Tasks;

namespace ChatBotRAG.Services.Interfaces
{
    public interface IChatService
    {
        Task<Guid> CreateSessionAsync(string title);
        Task<string> SendMessageAsync(Guid SessionId, string userMessage);
    }
}
