using LanguageLearningApp.Data;
using LanguageLearningApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanguageLearningApp.Controllers
{
    public class FiszkiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FiszkiController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        [HttpPost]
        public async Task<IActionResult> AddFlashcard(string TekstPolski, string TekstObcy)
        {
            if (string.IsNullOrWhiteSpace(TekstPolski) || string.IsNullOrWhiteSpace(TekstObcy))
            {
                TempData["Error"] = "Oba pola są wymagane!";
                return View();
            }
        
            var flashCard = new FlashCard
            {
                TekstPolski = TekstPolski,
                TekstObcy = TekstObcy,
                DataUtworzenia = DateTime.Now
            };
        
            _context.FlashCards.Add(flashCard);
            await _context.SaveChangesAsync();
        
            TempData["Success"] = "Fiszka została dodana!";
            return RedirectToAction("AdminPanelView", "Home");
        }
        // Pobierz fiszkę na podstawie ID (lub pierwszą, jeśli ID nie podano)
        public async Task<IActionResult> GetFlashcard(int? id)
        {
            FlashCard flashcard;

            if (id == null)
            {
                // Pobierz pierwszą fiszkę
                flashcard = await _context.FlashCards.OrderBy(f => f.Id).FirstOrDefaultAsync();
            }
            else
            {
                // Pobierz kolejną fiszkę na podstawie ID
                flashcard = await _context.FlashCards.Where(f => f.Id > id).OrderBy(f => f.Id).FirstOrDefaultAsync();
            }

            if (flashcard == null)
            {
                return Json(new { success = false, message = "Brak kolejnych fiszek do wyświetlenia." });
            }

            return Json(new
            {
                success = true,
                id = flashcard.Id,
                tekstPolski = flashcard.TekstPolski,
                tekstObcy = flashcard.TekstObcy
            });
        }
        [HttpGet]
        public async Task<IActionResult> GetAllFlashcards()
        {
            var flashcards = await _context.FlashCards
                .Select(f => new { tekstPolski = f.TekstPolski, tekstObcy = f.TekstObcy })
                .ToListAsync();

            return Json(flashcards);
        }


    }
}