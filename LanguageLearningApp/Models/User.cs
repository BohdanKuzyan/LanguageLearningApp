using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema; // Add this import for NotMapped

public class User
{
    [Key]
    public int Id { get; set; }

    [Required(ErrorMessage = "Imię jest wymagane.")]
    [StringLength(50, ErrorMessage = "Imię nie może przekraczać 50 znaków.")]
    public string Imie { get; set; }

    [Required(ErrorMessage = "Nazwisko jest wymagane.")]
    [StringLength(50, ErrorMessage = "Nazwisko nie może przekraczać 50 znaków.")]
    public string Nazwisko { get; set; }

    [Required(ErrorMessage = "Email jest wymagany.")]
    [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
    public string Email { get; set; }

    // This is the actual stored hashed password
    public byte[] Haslo { get; set; }

    [Required(ErrorMessage = "Hasło jest wymagane.")]
    [DataType(DataType.Password)]
    [MinLength(8, ErrorMessage = "Hasło musi mieć co najmniej 8 znaków.")]
    [RegularExpression(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).+$", ErrorMessage = "Hasło musi zawierać co najmniej jedną wielką literę, jedną małą literę i jedną cyfrę.")]
    [NotMapped] // This property will not be mapped to the database
    public string PlainTextPassword { get; set; }

    [Required(ErrorMessage = "Wybierz awatar.")]
    public string Awatar { get; set; } = "inne"; // Domyślny awatar ustawiony na "inne"
}




