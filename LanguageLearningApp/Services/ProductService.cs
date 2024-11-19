using LanguageLearningApp.Data;
using LanguageLearningApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LanguageLearningApp.Services;

public class ProductService
{
    private readonly ApplicationDbContext _context;

    public ProductService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }
}