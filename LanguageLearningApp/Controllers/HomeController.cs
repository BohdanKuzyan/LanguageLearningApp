using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;

namespace PolishLearningApp.Controllers
{
    using System.Linq;

    
    public class HomeController : Controller
    {
        // private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Lessons()
        {
            // Mock data for lessons to run without database
            var lessons = new List<Lesson>
            {
                new Lesson { Id = 1, Title = "Introduction to Polish", Content = "Learn the basics of Polish language." },
                new Lesson { Id = 2, Title = "Polish Grammar", Content = "Understand the grammar rules of Polish." },
                new Lesson { Id = 3, Title = "Common Polish Phrases", Content = "Learn commonly used phrases in Polish." }
            };
            return View(lessons);
        }

        public ActionResult LessonDetail(int id)
        {
            // Mock data for lesson detail
            var lesson = new Lesson { Id = id, Title = "Detailed Lesson", Content = "This is the detailed content of the lesson." };
            return View(lesson);
        }


        public IActionResult Privacy()
        {
            return View(Privacy);
        }
    }
}