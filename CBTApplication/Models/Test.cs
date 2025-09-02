using CBTApplication.Models;
using System.ComponentModel.DataAnnotations;


namespace CBTApplication.Models
{
    public class Test
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Test name is required.")]
        [StringLength(100, ErrorMessage = "Test name cannot exceed 100 characters.")]
        [Display(Name = "Test Name")]
        public string TestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 240, ErrorMessage = "Duration must be between 1 and 240 minutes.")]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Number of Questions")]
        public int NumberOfQuestions { get; set; }

        // QuestionBank relationship
        public int QuestionBankId { get; set; }

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        // Navigation properties
        public virtual QuestionBank QuestionBank { get; set; }
        public virtual ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        public virtual ICollection<TestDepartment> TestDepartments { get; set; } = new List<TestDepartment>();

        public int CurrentQuestionIndex { get; set; }
    }
}