using ChatBotRAG.DataAccess.Data;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.Threading.Tasks;

namespace ChatBotRAG.Web.Controllers
{
    public class AccountController : Controller
    {
        private readonly ChatBotDbContext _context;

        public AccountController(ChatBotDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                ViewData["Error"] = "Vui lòng nhập tài khoản và mật khẩu.";
                return View();
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == username);

            if (user == null || user.PasswordHash != password) // Simplified password check for demo
            {
                ViewData["Error"] = "Tài khoản hoặc mật khẩu không đúng.";
                return View();
            }

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role)
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var principal = new ClaimsPrincipal(identity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return LocalRedirect(returnUrl);
            }
            
            // Redirect based on role
            if (user.Role == "Admin")
                return RedirectToAction("Index", "Admin");
            else if (user.Role == "Lecturer")
                return RedirectToAction("Index", "Lecturer");
            else
                return RedirectToAction("Index", "Home"); // Student
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return RedirectToAction("Login");
        }
        
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}
