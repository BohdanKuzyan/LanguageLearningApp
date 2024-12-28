using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using LanguageLearningApp.Data;
using LanguageLearningApp.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LanguageLearningApp.Controllers
{
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
        public ActionResult RegisterView()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> RegisterView(User user, string confirmPassword, string activationCode)
        {
            ModelState.Clear();
            // Sprawdź, czy hasła są zgodne
            if (string.IsNullOrEmpty(user.PlainTextPassword) || user.PlainTextPassword != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Hasła nie są zgodne.");
            }

            // Walidacja wymagań dotyczących hasła
            if (string.IsNullOrEmpty(user.PlainTextPassword) ||
                user.PlainTextPassword.Length < 8 ||
                !user.PlainTextPassword.Any(char.IsUpper) ||
                !user.PlainTextPassword.Any(char.IsLower) ||
                !user.PlainTextPassword.Any(char.IsDigit))
            {
                ModelState.AddModelError("PlainTextPassword",
                    "Hasło musi mieć co najmniej 8 znaków, zawierać wielką literę, małą literę i cyfrę.");
            }

            // Sprawdzenie kodu aktywacyjnego
            if (string.IsNullOrEmpty(activationCode))
            {
                ModelState.AddModelError("ActivationCode", "Kod aktywacyjny jest wymagany.");
            }
            else
            {
                try
                {
                    var validCode = await _context.Kody_Rejestracji
                        .AsNoTracking()
                        .FirstOrDefaultAsync(k => k.KOD == activationCode);

                    if (validCode == null)
                    {
                        ModelState.AddModelError("ActivationCode", "Nieprawidłowy kod aktywacyjny.");
                    }
                    else
                    {
                        user.Typ = validCode.Typ;
                    }
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", $"Błąd połączenia z bazą danych: {ex.Message}");
                }
            }

            // Ostateczne sprawdzenie ModelState
            if (!ModelState.IsValid)
            {
                return View(user);
            }

            // Zapis użytkownika do bazy danych
            try
            {
                user.Haslo = Encoding.UTF8.GetBytes(user.PlainTextPassword);
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Rejestracja zakończona sukcesem.";
                return RedirectToAction("LoginView", "User");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Wystąpił problem podczas zapisu: {ex.Message}");
                return View(user);
            }
        }


        [AllowAnonymous]
        public ActionResult LoginView()
        {
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<ActionResult> LoginView(string email, string password)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                ModelState.AddModelError("", "Email i hasło są wymagane.");
                return View();
            }

            if (loginAttempts.TryGetValue(email, out var attemptInfo))
            {
                if (attemptInfo.Attempts >= 5 && attemptInfo.LastAttempt.AddMinutes(1) > DateTime.Now)
                {
                    ModelState.AddModelError("",
                        "Konto jest tymczasowo zablokowane z powodu wielu nieudanych prób logowania. Spróbuj ponownie później.");
                    return View();
                }
            }

            // Hash the input password
            byte[] hashedPassword = Encoding.UTF8.GetBytes(password);

            var user = await _context.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email && u.Haslo.SequenceEqual(hashedPassword));

            if (user != null)
            {
                loginAttempts.TryRemove(email, out _);

                // Tworzenie listy `Claim` dla zalogowanego użytkownika
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.Name, user.Email),
                    new Claim("Awatar", user.Awatar), // Claim dla awatara użytkownika
                    new Claim(ClaimTypes.Role, user.Typ) // Role ustawiona na wartość z kolumny Typ
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

                // Sprawdzenie typu użytkownika i przekierowanie na odpowiednią stronę
                if (user.Typ == "ADMIN")
                {
                    return RedirectToAction("AdminPanelView", "Home");
                }
                else if (user.Typ == "USER")
                {
                    return RedirectToAction("UserPanelView", "Home");
                }
                else
                {
                    // Domyślne przekierowanie, jeśli typ użytkownika nie jest określony
                    return RedirectToAction("LoginView", "User");
                }
            }
            else
            {
                var attempts = 1;
                if (loginAttempts.TryGetValue(email, out attemptInfo))
                {
                    attempts = attemptInfo.Attempts + 1;
                }

                loginAttempts[email] = (attempts, DateTime.Now);
                ModelState.AddModelError("", "Niepoprawny email lub hasło");
                return View();
            }
        }
        [HttpPost]
        [Authorize]
        public async Task<IActionResult> GenerateCodeToDb(string code, string userType)
        {
            if (string.IsNullOrEmpty(code) || string.IsNullOrEmpty(userType))
            {
                return BadRequest(new { message = "Kod lub typ użytkownika nie może być pusty." });
            }

            try
            {
                var newCode = new KodyRejestracji
                {
                    KOD = code,
                    Typ = userType
                };

                _context.Kody_Rejestracji.Add(newCode);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Kod został zapisany do bazy danych." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Błąd podczas zapisu do bazy danych: {ex.Message}" });
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
            return RedirectToAction("LoginView", "User");
        }
        [HttpDelete]
        [Authorize(Roles = "ADMIN")] // Tylko administrator może usuwać użytkowników
        public async Task<IActionResult> DeleteUser(int id)
        {
            try
            {
                // Znajdź użytkownika po ID
                var user = await _context.Users.FindAsync(id);

                if (user == null)
                {
                    return NotFound(new { message = "Użytkownik nie został znaleziony." });
                }

                // Usuń użytkownika z bazy danych
                _context.Users.Remove(user);
                await _context.SaveChangesAsync();

                return Ok(new { message = "Użytkownik został pomyślnie usunięty." });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Wystąpił błąd: {ex.Message}" });
            }
        }
        [HttpGet]
        [Authorize(Roles = "ADMIN")] // Tylko administrator ma dostęp
        public async Task<IActionResult> GetUsers()
        {
            try
            {
                var users = await _context.Users
                    .Select(user => new
                    {
                        user.Id,
                        user.Imie,
                        user.Nazwisko,
                        user.Email,
                        user.Typ
                    })
                    .ToListAsync();

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = $"Wystąpił błąd: {ex.Message}" });
            }
        }


    }
}