using System.ComponentModel.DataAnnotations;
    
public class Lesson
{
    [Key]
    public int Id { get; set; }
    public string Title { get; set; }
    public string Content { get; set; }
    public string Exercise { get; set; }
}
