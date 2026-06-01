using Microsoft.AspNetCore.Mvc;

namespace ChatBotRAG.Web.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
