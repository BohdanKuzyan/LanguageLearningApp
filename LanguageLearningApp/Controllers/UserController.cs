using Microsoft.AspNetCore.Authorization;

namespace PolishLearningApp.Controllers;

using LanguageLearningApp.Data;
using LanguageLearningApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http; // Dodaj ten import do obsługi sesji

public class UserController : Controller
{
    private readonly ApplicationDbContext _context;

    // ConcurrentDictionary służy do przechowywania liczby nieudanych prób logowania dla poszczególnych użytkowników.
    private static ConcurrentDictionary<string, (int Attempts, DateTime LastAttempt)> loginAttempts = new();

    public UserController(ApplicationDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }

    [AllowAnonymous]
    public ActionResult Register()
    {
        return View();
    }

    [HttpPost]
    public async Task<ActionResult> Register(User user)
    {
        if (ModelState.IsValid)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            TempData["Message"] = "User registered successfully.";
            return RedirectToAction("Index", "Home");
        }

        return View(user);
    }

    [AllowAnonymous]
    public ActionResult Login()
    {
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    public async Task<ActionResult> Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            ModelState.AddModelError("", "Username and password are required.");
            return View();
        }

        if (loginAttempts.TryGetValue(username, out var attemptInfo))
        {
            if (attemptInfo.Attempts >= 5 && attemptInfo.LastAttempt.AddMinutes(1) > DateTime.Now)
            {
                ModelState.AddModelError("",
                    "Account temporarily locked due to multiple failed login attempts. Please try again later.");
                return View();
            }
        }

        // Sprawdzenie użytkownika w bazie danych
        var user = await _context.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(u => u.Username == username && u.Password == password);

        if (user != null)
        {
            // Logowanie powiodło się - resetujemy licznik nieudanych prób
            loginAttempts.TryRemove(username, out _);

            // Utworzenie listy roszczeń (claims) dla użytkownika
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, "User") // Możesz dodać więcej roszczeń, jeśli to konieczne
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

            // Logowanie użytkownika przy użyciu plików cookie
            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                {
                    IsPersistent =
                        true, // Logowanie trwałe (użytkownik nie musi się logować po zamknięciu przeglądarki)
                    ExpiresUtc = DateTime.UtcNow.AddMinutes(30) // Czas trwania logowania
                });

            return RedirectToAction("Index", "Home");
        }
        else
        {
            var attempts = 1;
            if (loginAttempts.TryGetValue(username, out attemptInfo))
            {
                attempts = attemptInfo.Attempts + 1;
            }

            loginAttempts[username] = (attempts, DateTime.Now);

            ModelState.AddModelError("", "Invalid username or password");
            return View();
        }
    }

    [HttpPost]
    [Authorize]
    public async Task<IActionResult> Logout()
    {
        // Wylogowanie użytkownika z autoryzacji opartej na ciasteczkach
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        // Usunięcie całej sesji
        HttpContext.Session.Clear();

        // Przekierowanie na widok logowania
        return RedirectToAction("Login", "User");
    }
}