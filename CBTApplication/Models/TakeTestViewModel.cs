using System.ComponentModel.DataAnnotations;

namespace CBTApplication.Models
{
    public class TakeTestViewModel
    {
        public int TestAssignmentId { get; set; }
        public int TestId { get; set; }
        public string TestName { get; set; } = string.Empty;
        public int CurrentQuestionIndex { get; set; }
        public int NumberOfQuestions { get; set; }
        public int DurationMinutes { get; set; }
        public DateTime? EndTime { get; set; }
        public int TimeRemainingSeconds { get; set; }
        public List<QuestionViewModel>? Questions { get; set; } 
        public string? SelectedOption { get; set; }
        public string? Action { get; set; }

        public Guid TestAttemptId { get; set; } // Added to track the current test attempt


    }

  
}