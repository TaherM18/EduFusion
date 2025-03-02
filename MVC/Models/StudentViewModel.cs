using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVC.Models
{
    public class StudentViewModel
    {
        public string StudentName { get; set; }
        public string Class { get; set; }

        public List<string> Classes { get; set; }
        public List<string> Subjects { get; set; }

        public List<string> AvailableSubjects { get; set; }
        public Dictionary<string, int> PerformanceSummary { get; set; } // Subject-wise performance
        public List<Exam> UpcomingExams { get; set; }
        public Dictionary<string, int> SyllabusProgress { get; set; } // Topic-wise syllabus completion
        public string PerformanceSummaryJson { get; set; }
    }

    public class Exam
    {
        public string Subject { get; set; }
        public string Date { get; set; }
    }
}