using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
public class User
{
    [Key]
    public int UserId { get; set; }
    [Required]
    public string Username { get; set; }
    [Required]
    [DataType(DataType.Password)]
    public string Password { get; set; }
    public List<Lesson> CompletedLessons { get; set; }
}