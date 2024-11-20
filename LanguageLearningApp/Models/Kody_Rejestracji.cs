namespace LanguageLearningApp.Models;
using System.ComponentModel.DataAnnotations;
public class Kody_Rejestracji
{
    [Key]
    public int Id { get; set; }
    [Required]
    public string KOD { get; set; }
}