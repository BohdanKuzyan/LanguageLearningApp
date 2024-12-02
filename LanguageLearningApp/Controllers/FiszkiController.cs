using LanguageLearningApp.Data;
using LanguageLearningApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PolishLearningApp.Controllers
{
    public class FiszkiController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FiszkiController(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        // Akcja, która zwraca widok FlashcardsView z listą działów
        public async Task<IActionResult> FlashcardsView()
        {
            // Pobranie wszystkich działów z bazy danych
            var dzialy = await _context.Dzialy.ToListAsync();

            // Jeśli `dzialy` jest `null`, przypisz pustą listę, aby uniknąć błędu
            if (dzialy == null)
            {
                dzialy = new List<Dzial>();
            }

            return View(dzialy); // Zwrócenie widoku z modelem `dzialy`
        }
    }
}