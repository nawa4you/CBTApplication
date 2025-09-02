using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTApplication.Models
{
    public class TestAttempt
    {


        public Guid Id { get; set; } = Guid.NewGuid();

        [Required]
        public string UserId { get; set; } = string.Empty; // Foreign key to User.Email

        [Required]
        public int TestId { get; set; } // Foreign key to Test

        [Display(Name = "Start Time")]
        public DateTime StartTime { get; set; } = DateTime.UtcNow;

        [Display(Name = "End Time")]
        public DateTime? EndTime { get; set; } // Nullable, set upon completion

        [Display(Name = "Score")]
        public int? Score { get; set; } // Nullable, set upon completion
      

        [Display(Name = "Is Completed")]
        public bool IsCompleted { get; set; } = false;

          //for the shuffled questions
        //public string? AssignedQuestionOrderJson { get; set; }

        // Navigation properties
        [ForeignKey("UserId")]
        public User? User { get; set; }

        [ForeignKey("TestId")]
        public Test? Test { get; set; }

        //public int TimeRemainingSeconds { get; set; }
        public DateTime DateAttempted { get; set; } = DateTime.Now;


        public int NumberOfQuestions { get; set; }


        public int CurrentQuestionIndex { get; set; } = 0;

        public Department? Department { get; set; }

        public ICollection<QuestionOrderNumber> QuestionOrderNumbers { get; set; } = new List<QuestionOrderNumber>();








        public ICollection<StudentAnswer> StudentAnswers { get; set; } = new List<StudentAnswer>();




























    }
}