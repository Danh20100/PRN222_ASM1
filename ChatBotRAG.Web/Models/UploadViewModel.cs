using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace ChatBotRAG.Web.Models
{
    public class UploadViewModel
    {
        [Required(ErrorMessage = "Vui lòng chọn file.")]
        public IFormFile File { get; set; }
        
        public string? Message { get; set; }
        public bool IsSuccess { get; set; }
    }
}
