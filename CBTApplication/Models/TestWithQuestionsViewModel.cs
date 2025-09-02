using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CBTApplication.Models; 

namespace CBTApplication.Models
{
    public class TestWithQuestionsViewModel
    {
        // Properties for the Test itself
        public int Id { get; set; } 

        [Required(ErrorMessage = "Test name is required.")]
        [StringLength(100, ErrorMessage = "Test name cannot exceed 100 characters.")]
        [Display(Name = "Test Name")]
        public string TestName { get; set; } = string.Empty;
        public int? TestId { get; set; }


        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 240, ErrorMessage = "Duration must be between 1 and 240 minutes.")]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Display(Name = "Is Active")]
        public bool IsActive { get; set; } = true;

        [Required(ErrorMessage = "Number of questions is required.")]
        [Range(1, 100, ErrorMessage = "Number of questions must be between 1 and 100.")]
        [Display(Name = "Number of Questions")]
        public int NumberOfQuestions { get; set; }

        public System.DateTime DateCreated { get; set; } = System.DateTime.UtcNow;
         
        public List<QuestionViewModel> Questions { get; set; } = new List<QuestionViewModel>();


         
        public QuestionViewModel NewQuestion { get; set; } = new QuestionViewModel();
    }


}
