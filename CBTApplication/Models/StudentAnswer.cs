using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTApplication.Models
{
    public class StudentAnswer
    {
        public int Id { get; set; }

        [Required]
        public Guid TestAttemptId { get; set; } // Foreign key to TestAttempt

        [Required]
        public int QuestionId { get; set; } // Foreign key to Question

        [StringLength(1, ErrorMessage = "Selected option must be A, B, C, or D.")]
        [RegularExpression("^[A-D]$", ErrorMessage = "Selected option must be A, B, C, or D.")]
        [Display(Name = "Selected Option")]
        public string? SelectedOption { get; set; } // The option selected by the student (A, B, C, D, or null if not answered)

        [Display(Name = "Is Correct")]
        public bool IsCorrect { get; set; } = false; // Nullable, set after test submission

        [Required]
        public int OrderIndex { get; set; }

        // Navigation properties
        [ForeignKey("TestAttemptId")]
        public TestAttempt? TestAttempt { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}
