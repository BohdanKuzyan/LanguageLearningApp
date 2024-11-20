using LanguageLearningApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Dodaj sesję do usługi
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30); // Czas, przez jaki sesja jest ważna (np. 30 minut)
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Dodaj uwierzytelnianie (Authentication) i autoryzację (Authorization)
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/Login";
        options.LogoutPath = "/User/Logout"; // Dodano ścieżkę wylogowania
        options.AccessDeniedPath = "/User/Login"; // Gdzie przekierować po odmowie dostępu
    });

// Dodaj kontrolery z widokami oraz globalną politykę autoryzacji
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => { policy.RequireAuthenticatedUser(); });
});

// Dodajemy autoryzację jako globalną politykę
builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter("RequireAuthenticatedUser"));
});

// Dodanie ApplicationDbContext do DI
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("ManufakturaSukcesuDB")));

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// Użyj sesji w aplikacji
app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); // Dodaj to przed UseAuthorization
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=Login}/{id?}");

app.Run();