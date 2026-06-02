using ChatBotRAG.Services.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace ChatBotRAG.Web.Controllers
{
    [Authorize]
    public class ChatController : Controller
    {
        private readonly IChatService _chatService;
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _environment;

        public ChatController(IChatService chatService, IDocumentService documentService, IWebHostEnvironment environment)
        {
            _chatService = chatService;
            _documentService = documentService;
            _environment = environment;
        }

        [HttpPost]
        public async Task<IActionResult> SendMessage([FromForm] ChatMessageRequest request)
        {
            if (request == null)
                return BadRequest();

            try
            {
                // 1. Ensure Session
                if (request.SessionId == Guid.Empty)
                {
                    request.SessionId = await _chatService.CreateSessionAsync("Phiên chat " + DateTime.Now.ToString("dd/MM/yyyy HH:mm"));
                }

                string systemMessage = "";

                // 2. Process File if attached
                if (request.File != null && request.File.Length > 0)
                {
                    var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
                    if (!Directory.Exists(uploadsFolder))
                        Directory.CreateDirectory(uploadsFolder);

                    var filePath = Path.Combine(uploadsFolder, request.File.FileName);
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.File.CopyToAsync(stream);
                    }

                    // Chunk & Embed local process
                    Guid docSubjectId = request.SubjectId ?? Guid.Parse("11111111-1111-1111-1111-111111111111");
                    bool success = await _documentService.ProcessFileAsync(filePath, docSubjectId);
                    
                    if (success)
                        systemMessage += $"[Hệ thống: Đã xử lý và lưu trữ tài liệu '{request.File.FileName}' thành công] ";
                    else
                        systemMessage += $"[Hệ thống: Xử lý tài liệu '{request.File.FileName}' thất bại] ";
                }

                // 3. Process Message if any
                string responseText = systemMessage;
                if (!string.IsNullOrWhiteSpace(request.Message))
                {
                    var botAnswer = await _chatService.SendMessageAsync(request.SessionId, request.Message);
                    responseText += (string.IsNullOrEmpty(systemMessage) ? "" : "\n\n") + botAnswer;
                }

                if (string.IsNullOrWhiteSpace(responseText))
                {
                    responseText = "Bạn đã không gửi tài liệu hay câu hỏi nào.";
                }

                return Json(new { sessionId = request.SessionId, response = responseText });
            }
            catch (Exception ex)
            {
                string errorMsg = ex.Message;
                if (ex.InnerException != null) 
                {
                    errorMsg += " -> Chi tiết lỗi gốc: " + ex.InnerException.Message;
                }
                return Json(new { sessionId = request.SessionId, response = $"Lỗi hệ thống: {errorMsg}" });
            }
        }
    }

    public class ChatMessageRequest
    {
        public Guid SessionId { get; set; }
        public string? Message { get; set; }
        public IFormFile? File { get; set; }
        public Guid? SubjectId { get; set; } // Giới hạn phạm vi
    }
}
