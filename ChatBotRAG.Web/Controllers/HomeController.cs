using Microsoft.AspNetCore.Mvc;

using Microsoft.AspNetCore.Authorization;

using ChatBotRAG.DataAccess.Models;
using ChatBotRAG.DataAccess.Repositories;
using System.Threading.Tasks;

namespace ChatBotRAG.Web.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        private readonly IRepository<Subject> _subjectRepository;

        public HomeController(IRepository<Subject> subjectRepository)
        {
            _subjectRepository = subjectRepository;
        }

        public async Task<IActionResult> Index()
        {
            if (User.IsInRole("Admin") || User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "Admin");
            }
            if (User.IsInRole("Lecturer") || User.IsInRole("lecturer"))
            {
                return RedirectToAction("Index", "Lecturer");
            }

            var subjects = await _subjectRepository.GetAllAsync();
            return View(subjects);
        }
    }
}
