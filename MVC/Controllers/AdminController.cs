using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    public class AdminController : Controller
    {
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Materials()
        {
            return View();
        }


        public IActionResult Students()
        {
            return View();
        }

        public IActionResult Subjects()
        {
            return View();
        }

        public IActionResult Profile()
        {
            return View();
        }

        public IActionResult Teachers()
        {
            return View();
        }

        public IActionResult Notifications()
        {
            return View();
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}