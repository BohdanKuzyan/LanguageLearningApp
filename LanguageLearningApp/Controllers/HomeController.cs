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
        public IActionResult Privacy()
        {
            return View(Privacy);
        }
    }
}