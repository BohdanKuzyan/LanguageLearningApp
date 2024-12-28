using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageLearningApp.Models
{
    public class FlashCard
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(255)]
        public string TekstPolski { get; set; }

        [Required]
        [StringLength(255)]
        public string TekstObcy { get; set; }

        public DateTime DataUtworzenia { get; set; } = DateTime.Now;
    }

}