using LanguageLearningApp.Data;
using LanguageLearningApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LanguageLearningApp.Services;

public class UserService
{
    private readonly ApplicationDbContext _context;

    public UserService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<User>> GetAllUsersAsync()
    {
        return await _context.Users.ToListAsync();
    }
}