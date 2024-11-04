using Microsoft.AspNetCore.Mvc;

namespace PolishLearningApp.Controllers
{
    using System.Linq;
    
    public class UserController : Controller
    {
        // private ApplicationDbContext db = new ApplicationDbContext();

        public ActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Register(User user)
        {
            if (ModelState.IsValid)
            {
                // Mock registration logic
                TempData["Message"] = "User registered successfully.";
                return RedirectToAction("Index", "Home");
            }
            return View(user);
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        public ActionResult Login(string username, string password)
        {
            // Mock login logic
            if (username == "test" && password == "password")
            {
                return RedirectToAction("Lessons", "Home");
            }
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }
    }
}
