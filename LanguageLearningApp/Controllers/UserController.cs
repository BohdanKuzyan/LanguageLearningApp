using Microsoft.AspNetCore.Authorization;
using LanguageLearningApp.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Concurrent;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;


namespace PolishLearningApp.Controllers
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
            // Validate if passwords match
            if (user.PlainTextPassword != confirmPassword)
            {
                ModelState.AddModelError("ConfirmPassword", "Hasła nie są zgodne.");
            }

            // Validate the password requirements
            if (string.IsNullOrEmpty(user.PlainTextPassword) ||
                user.PlainTextPassword.Length < 8 ||
                !user.PlainTextPassword.Any(char.IsUpper) ||
                !user.PlainTextPassword.Any(char.IsLower) ||
                !user.PlainTextPassword.Any(char.IsDigit))
            {
                ModelState.AddModelError("PlainTextPassword",
                    "Hasło musi mieć co najmniej 8 znaków, zawierać wielką literę, małą literę i cyfrę.");
            }

            // Validate the activation code
            if (string.IsNullOrEmpty(activationCode))
            {
                ModelState.AddModelError("ActivationCode", "Kod aktywacyjny jest wymagany.");
            }

            // Check if the activation code is valid
            var validCode = await _context.Kody_Rejestracji
                .AsNoTracking()
                .FirstOrDefaultAsync(k => k.KOD == activationCode);

            if (validCode == null)
            {
                ModelState.AddModelError("ActivationCode", "Nieprawidłowy kod aktywacyjny.");
            }

            // If all is valid, convert the password and save
            if (ModelState.IsValid)
            {
                // Convert the password to byte[] before saving
                user.Haslo = System.Text.Encoding.UTF8.GetBytes(user.PlainTextPassword);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                TempData["Message"] = "Rejestracja zakończona sukcesem.";
                return RedirectToAction("LoginView", "User");
            }

            // If there are validation errors, return to the view
            return View(user);
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
            byte[] hashedPassword;
            hashedPassword = Encoding.UTF8.GetBytes(password);

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
                    new Claim(ClaimTypes.Role, "User")
                };

                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);

                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme,
                    new ClaimsPrincipal(claimsIdentity), new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(30)
                    });

                return RedirectToAction("UserPanelView", "Home");
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
        public async Task<IActionResult> Logout()
        {
            // Wylogowanie użytkownika z autoryzacji opartej na ciasteczkach
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

            // Usunięcie całej sesji
            HttpContext.Session.Clear();

            // Przekierowanie na widok logowania
            return RedirectToAction("LoginView", "User");
        }
    }
}