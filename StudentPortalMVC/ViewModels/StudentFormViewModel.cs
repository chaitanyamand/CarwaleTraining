using System.ComponentModel.DataAnnotations;

namespace StudentPortal.ViewModels
{
    public class StudentFormViewModel
    {
        [Required]
        public string Name { get; set; }

        [Required]
        public string Course { get; set; }

        public string PageTitle { get; set; } = "Create New Student";
    }
}
