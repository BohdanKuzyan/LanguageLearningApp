using System;
using System.ComponentModel.DataAnnotations;

namespace LanguageLearningApp.Models
{
    public class KodyRejestracji
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string KOD { get; set; }
        
        public DateTime Data_dodania { get; set; }

        [Required]
        public string Typ { get; set; }
    }
}