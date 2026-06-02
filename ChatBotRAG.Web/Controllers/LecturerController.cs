using ChatBotRAG.DataAccess.Models;
using ChatBotRAG.DataAccess.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace ChatBotRAG.Web.Controllers
{
    [Authorize(Roles = "Lecturer,lecturer")]
    public class LecturerController : Controller
    {
        private readonly IRepository<Subject> _subjectRepository;
        private readonly ChatBotRAG.DataAccess.Data.ChatBotDbContext _context;

        public LecturerController(IRepository<Subject> subjectRepository, ChatBotRAG.DataAccess.Data.ChatBotDbContext context)
        {
            _subjectRepository = subjectRepository;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var subjects = await _subjectRepository.GetAllAsync();
            return View(subjects);
        }

        public async Task<IActionResult> Subjects()
        {
            var subjectsWithDocs = await Microsoft.EntityFrameworkCore.EntityFrameworkQueryableExtensions.Include(_context.Subjects, s => s.Documents).ToListAsync();
            return View(subjectsWithDocs);
        }

        public IActionResult ChatLogs()
        {
            // TODO: View student chat sessions
            return View();
        }
    }
}
