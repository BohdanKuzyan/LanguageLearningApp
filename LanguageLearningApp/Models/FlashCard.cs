using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace LanguageLearningApp.Models
{
    public class Flashcard
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Napis po polsku jest wymagany.")]
        [StringLength(200, ErrorMessage = "Napis nie może przekraczać 200 znaków.")]
        public string NapisPolski { get; set; } // Napis po polsku

        [Required(ErrorMessage = "Napis po ukraińsku i rosyjsku jest wymagany.")]
        [StringLength(200, ErrorMessage = "Napis nie może przekraczać 200 znaków.")]
        public string NapisUkraRos { get; set; } // Napis po ukraińsku i rosyjsku

        [Required]
        public int DzialId { get; set; } // Id działu, do którego należy fiszka

        [ForeignKey("DzialId")]
        public Dzial Dzial { get; set; } // Nawigacja do działu, umożliwia powiązanie fiszki z działem
    }
}