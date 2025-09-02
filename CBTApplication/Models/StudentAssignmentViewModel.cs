using System.ComponentModel.DataAnnotations;

namespace CBTApplication.Models
{
    public class StudentAssignmentViewModel
    {
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Student Name")]
        public string UserName { get; set; } = string.Empty;

        [Display(Name = "Assign Test")]
        public bool IsAssigned { get; set; }
    }
}
