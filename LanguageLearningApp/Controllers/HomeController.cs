using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace PolishLearningApp.Controllers
{
    
    public class HomeController : Controller
    {
        [Authorize]
        public IActionResult UserPanelView()
        {
            return View();
        }
        
        [AllowAnonymous]
        public IActionResult PrivacyView()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult AdminPanelView()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult FlashcardsView()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult GrammarPDFView()
        {
            return View();
        }
        
        [Authorize]
        public IActionResult QuizView()
        {
            return View();
        }
        
        public IActionResult SetUkrainianLayout()
        {
            // Ustawienie w sesji informacji, aby używać układu _MainLayoutUA
            HttpContext.Session.SetString("Layout", "_MainLayoutUA");

            // Przekierowanie z powrotem do panelu użytkownika
            return RedirectToAction("UserPanelView");
        }
        
        public IActionResult SetPolishLayout()
        {
            // Ustawienie w sesji informacji, aby używać układu _MainLayoutUA
            HttpContext.Session.SetString("Layout", "_Layout");

            // Przekierowanie z powrotem do panelu użytkownika
            return RedirectToAction("UserPanelView");
        }
        
    }
}