using LanguageLearningApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace PolishLearningApp.Controllers
{
    using System.Linq;
    
    public class UserController : Controller
    {
        private readonly ApplicationDbContext _context;
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
                return RedirectToAction("Privacy", "Home");
            }
            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }
        public async Task<IActionResult> CheckDatabaseConnection()
        {
            try
            {
                // Sprawdzenie połączenia z bazą danych - np. proste zapytanie do tabeli `Products`.
                var products = await _context.Products.FirstOrDefaultAsync();
                if (products != null)
                {
                    ViewBag.Message = "Połączenie z bazą danych działa poprawnie.";
                }
                else
                {
                    ViewBag.Message = "Połączenie z bazą danych działa, ale brak danych w tabeli 'Products'.";
                }
            }
            catch (Exception ex)
            {
                ViewBag.Message = $"Błąd połączenia z bazą danych: {ex.Message}";
            }

            return View();
        }
    }
}
