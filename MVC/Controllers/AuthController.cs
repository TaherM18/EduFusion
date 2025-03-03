using Microsoft.AspNetCore.Mvc;

namespace MVC.Controllers
{
    public class AuthController : Controller
    {
        public IActionResult Login()
        {
            return View();
        }

        public IActionResult Logout()
        {
            return View();
        }
    }
}