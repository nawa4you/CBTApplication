using CBTApplication.Models;
using System;
using System.Collections.Generic; // Added for ICollection
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTApplication.Models
{
    public class AssignTest
    {
        public int Id { get; set; }

        [Required]
        public int TestId { get; set; }

        [Required]
        public string UserId { get; set; } = string.Empty;

        [Display(Name = "Assigned Date")]
        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; } = false;

        [Display(Name = "Score")]
        public int? Score { get; set; }

        [Display(Name = "Submission Date")]
        public DateTime? SubmissionDate { get; set; }

        [Display(Name = "Assigned Question Order")]
        public string? AssignedQuestionOrderJson { get; set; }

        [ForeignKey("TestId")]
        public Test? Test { get; set; }

        [ForeignKey("UserId")]
        public User? User { get; set; }

        
        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();
    }
}
