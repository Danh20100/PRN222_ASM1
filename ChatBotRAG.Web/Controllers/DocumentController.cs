using ChatBotRAG.Services.Interfaces;
using ChatBotRAG.Web.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Authorization;

namespace ChatBotRAG.Web.Controllers
{
    [Authorize(Roles = "Admin,admin,Lecturer,lecturer")]
    public class DocumentController : Controller
    {
        private readonly IDocumentService _documentService;
        private readonly IWebHostEnvironment _environment;

        public DocumentController(IDocumentService documentService, IWebHostEnvironment environment)
        {
            _documentService = documentService;
            _environment = environment;
        }

        [HttpGet]
        public IActionResult Upload()
        {
            return View(new UploadViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Upload(UploadViewModel model)
        {
            if (!ModelState.IsValid || model.File == null)
            {
                model.Message = "File không hợp lệ.";
                model.IsSuccess = false;
                return View(model);
            }

            // Save file temporarily
            var uploadsFolder = Path.Combine(_environment.WebRootPath, "uploads");
            if (!Directory.Exists(uploadsFolder))
                Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, model.File.FileName);
            
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await model.File.CopyToAsync(stream);
            }

            // Process file via Business Layer
            bool success = await _documentService.ProcessFileAsync(filePath, Guid.Parse("11111111-1111-1111-1111-111111111111")); // Mock SubjectId 1

            model.IsSuccess = success;
            if (success)
            {
                model.Message = "Tải lên, phân tích và lưu trữ tài liệu (chunking) thành công!";
            }
            else
            {
                model.Message = "Lỗi trong quá trình xử lý tài liệu. Chỉ hỗ trợ pdf, docx, txt.";
            }

            return View(model);
        }
    }
}
