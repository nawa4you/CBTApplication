using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTApplication.Models
{
    public class QuestionOrderNumber
    {
        public int Id { get; set; }

        [Required]
        public Guid TestAttemptId { get; set; } // Foreign key to TestAttempt

        [Required]
        public int OrderIndex { get; set; } // 0-based index for question order

        [Required]
        public int QuestionId { get; set; } // Foreign key to Question

        [StringLength(1)]
        [RegularExpression("^[A-D]$", ErrorMessage = "Selected option must be A, B, C, or D.")]
        [Display(Name = "Selected Option")]
        public string? SelectedOption { get; set; } // Null by default

        [Display(Name = "Is Correct")]
        public bool IsCorrect { get; set; } = false; // Default to false

        // Navigation properties
        [ForeignKey("TestAttemptId")]
        public TestAttempt? TestAttempt { get; set; }

        [ForeignKey("QuestionId")]
        public Question? Question { get; set; }
    }
}