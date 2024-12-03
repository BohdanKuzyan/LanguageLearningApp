using LanguageLearningApp.Models;

namespace LanguageLearningApp.Data;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<KodyRejestracji> Kody_Rejestracji { get; set; }
    public DbSet<Dzial> Dzialy { get; set; }
    public DbSet<Flashcard> Flashcards { get; set; }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<KodyRejestracji>(entity =>
        {
            entity.Property(e => e.Data_dodania)
                .HasColumnType("DATE"); // Wymuszenie typu DATE w bazie danych
        });

        base.OnModelCreating(modelBuilder);
    }
    
}

