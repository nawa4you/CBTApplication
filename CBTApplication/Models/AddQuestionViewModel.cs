using System.ComponentModel.DataAnnotations;

namespace CBTApplication.Models
{
    public class AddQuestionViewModel
    {
        






        public int TestId { get; set; }

        //[Required]
        [Display(Name = "Test Name")]
        public string TestName { get; set; } = string.Empty;

        public List<Question> Questions { get; set; } = new List<Question>();



        [Required(ErrorMessage = "Question text is required.")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option A is required.")]
        public string OptionA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option B is required.")]
        public string OptionB { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option C is required.")]
        public string OptionC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option D is required.")]
        public string OptionD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Correct option is required.")]
        public string CorrectOption { get; set; } = string.Empty;

    }
}