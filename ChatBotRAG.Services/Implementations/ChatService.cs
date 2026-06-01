using ChatBotRAG.DataAccess.Models;
using ChatBotRAG.DataAccess.Repositories;
using ChatBotRAG.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ChatBotRAG.Services.Implementations
{
    public class ChatService : IChatService
    {
        private readonly IRepository<ChatSession> _sessionRepository;
        private readonly IRepository<ChatMessage> _messageRepository;
        private readonly IRepository<DocumentChunk> _chunkRepository;
        private readonly IRepository<MessageCitation> _citationRepository;

        public ChatService(
            IRepository<ChatSession> sessionRepository,
            IRepository<ChatMessage> messageRepository,
            IRepository<DocumentChunk> chunkRepository,
            IRepository<MessageCitation> citationRepository)
        {
            _sessionRepository = sessionRepository;
            _messageRepository = messageRepository;
            _chunkRepository = chunkRepository;
            _citationRepository = citationRepository;
        }

        public async Task<Guid> CreateSessionAsync(string title)
        {
            var session = new ChatSession
            {
                Title = title,
                CreatedAt = DateTime.UtcNow
            };
            await _sessionRepository.AddAsync(session);
            return session.Id;
        }

        public async Task<string> SendMessageAsync(Guid sid, string userMessage)
        {
            // 1. Lưu tin nhắn người dùng
            var userMsg = new ChatMessage
            {
                SessionId = sid,
                Role = "user",
                Content = userMessage,
                CreatedAt = DateTime.UtcNow
            };
            await _messageRepository.AddAsync(userMsg);

            // 2. Tìm kiếm (Giả lập vector search bằng like/contains do không dùng API ngoài)
            // Lấy ra tất cả chunks (trong thực tế sẽ dùng vector search ở SQL hoặc external DB)
            var allChunks = await _chunkRepository.GetAllAsync();
            var keywords = userMessage.ToLower().Split(' ', StringSplitOptions.RemoveEmptyEntries);
            
            // Tìm chunk có chứa nhiều keyword nhất (Simulated retrieval)
            var bestChunk = allChunks
                .OrderByDescending(c => keywords.Count(k => c.Content.ToLower().Contains(k)))
                .FirstOrDefault();

            string botResponse = "";
            DocumentChunk? citedChunk = null;

            if (bestChunk != null && keywords.Any(k => bestChunk.Content.ToLower().Contains(k)))
            {
                botResponse = $"[Dựa trên tài liệu] Tôi tìm thấy thông tin sau:\n{bestChunk.Content.Substring(0, Math.Min(bestChunk.Content.Length, 300))}...";
                citedChunk = bestChunk;
            }
            else
            {
                botResponse = "Xin lỗi, tôi không tìm thấy thông tin liên quan trong tài liệu môn học.";
            }

            // 3. Lưu tin nhắn bot
            var botMsg = new ChatMessage
            {
                SessionId = sid,
                Role = "assistant",
                Content = botResponse,
                CreatedAt = DateTime.UtcNow
            };
            await _messageRepository.AddAsync(botMsg);

            // 4. Lưu citation
            if (citedChunk != null)
            {
                var citation = new MessageCitation
                {
                    MessageId = botMsg.Id,
                    ChunkId = citedChunk.Id
                };
                await _citationRepository.AddAsync(citation);
            }

            return botResponse;
        }
    }
}
