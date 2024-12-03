using LanguageLearningApp.Data;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Authorization;

var builder = WebApplication.CreateBuilder(args);

// Dodaj IHttpContextAccessor do Dependency Injection
builder.Services.AddHttpContextAccessor();

// Dodaj sesję do usługi
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Dodaj uwierzytelnianie i autoryzację
builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/User/LoginView";
        options.LogoutPath = "/User/Logout";
        options.AccessDeniedPath = "/User/LoginView";
    });

// Dodaj autoryzację i kontrolery z widokami
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("RequireAuthenticatedUser", policy => { policy.RequireAuthenticatedUser(); });
});

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add(new AuthorizeFilter("RequireAuthenticatedUser"));
});

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseMySql(
        builder.Configuration.GetConnectionString("ManufakturaSukcesuDB"),
        new MySqlServerVersion(new Version(8, 4, 3)), // Ustaw wersję zgodną z Twoim serwerem MySQL
        mySqlOptions => mySqlOptions.EnableRetryOnFailure(5, TimeSpan.FromSeconds(10), null) // Dodano obsługę retry
    ));


var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Shared/Error");
    app.UseHsts();
}

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=User}/{action=LoginView}/{id?}");

app.Run();
