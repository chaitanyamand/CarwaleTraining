using System.ComponentModel.DataAnnotations;

namespace StudentPortal.Models
{
    public class Student
{
    public int Id { get; set; }

    [Required]
    public string Name { get; set; }

    [Required]
    public string Course { get; set; }
}
}
