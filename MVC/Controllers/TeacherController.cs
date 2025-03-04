using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace MVC.Controllers
{
    public class TeacherController : Controller
    {
        private readonly ILogger<TeacherController> _logger;

        public TeacherController(ILogger<TeacherController> logger)
        {
            _logger = logger;
        }

        public IActionResult Register()
        {
            return View();
        }

        public IActionResult Dashboard()
        {
            return View();
        }
        public IActionResult Classes()
        {
            return View();
        }

        public JsonResult GetTeacherSchedule()
        {
            var schedule = new List<object>
            {
                new { Subject = "Mathematics", Class = "10th A", Time = "10:00 AM - 11:00 AM" },
                new { Subject = "Physics", Class = "12th Science", Time = "11:30 AM - 12:30 PM" }
            };
            return Json(schedule);
        }

        public JsonResult GetStudentProgress()
        {
            var progress = new List<object>
            {
                new { Student = "John Doe", Subject = "Mathematics", Score = 85 },
                new { Student = "Noah Adams", Subject = "Mathematics", Score = 30 },
                new { Student = "Loretta Hicks", Subject = "Mathematics", Score = 46 },
                new { Student = "Marie Griffith", Subject = "Mathematics", Score = 70 },
                new { Student = "Joe McDonald", Subject = "Mathematics", Score = 80 },
                new { Student = "Mayme Greer", Subject = "Mathematics", Score = 79 },
                new { Student = "Alice Smith", Subject = "Physics", Score = 90 }
            };
            return Json(progress);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}