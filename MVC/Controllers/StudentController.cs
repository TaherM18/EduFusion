using System.Text.Json;
using Microsoft.AspNetCore.Mvc;
using MVC.Models;

namespace MVC.Controllers
{
    public class StudentController : Controller
    {
        private readonly ILogger<StudentController> _logger;

        public StudentController(ILogger<StudentController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            var student = new StudentViewModel
            {
                StudentName = "John Doe",
                Class = "10th Grade",
                Classes = new List<string> { "9th Grade", "10th Grade", "11th Grade", "12th Grade" },
                Subjects = new List<string> { "Mathematics", "Science", "English" },
                AvailableSubjects = new List<string> { "Mathematics", "Science", "English", "History", "Geography", "Physics", "Chemistry" },
                PerformanceSummary = new Dictionary<string, int>
                {
                    { "Mathematics", 85 },
                    { "Science", 78 },
                    { "English", 92 }
                },
                PerformanceSummaryJson = JsonSerializer.Serialize(new List<object>
                {
                    new { category = "Math", value = 85 },
                    new { category = "Science", value = 90 },
                    new { category = "English", value = 75 }
                }),

                UpcomingExams = new List<Exam>
                {
                    new Exam { Subject = "Mathematics", Date = "2025-03-10" },
                    new Exam { Subject = "Science", Date = "2025-03-15" }
                },
                SyllabusProgress = new Dictionary<string, int>
                {
                    { "Algebra", 70 },
                    { "Physics", 50 },
                    { "Grammar", 90 }
                }
            };
            return View(student);
        }


        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View("Error!");
        }
    }
}