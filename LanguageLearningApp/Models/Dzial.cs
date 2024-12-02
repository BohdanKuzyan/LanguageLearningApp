using System.ComponentModel.DataAnnotations;

namespace LanguageLearningApp.Models;

public class Dzial
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string Nazwa { get; set; }
}