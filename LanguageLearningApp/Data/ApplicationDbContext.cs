﻿using LanguageLearningApp.Models;

namespace LanguageLearningApp.Data;

using Microsoft.EntityFrameworkCore;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<KodyRejestracji> Kody_Rejestracji { get; set; }
    public DbSet<Dzial> Dzialy { get; set; }
    public DbSet<FlashCard> FlashCards { get; set; }
    
}

