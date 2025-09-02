using System.ComponentModel.DataAnnotations;

namespace CBTApplication.Models
{
    public class QuestionViewModel
    {
        public int Id { get; set; }
        public int TestId { get; set; }
        public int QuestionNumber { get; set; }
        public bool IsDeleted { get; set; } = false;
        public int NumberOfQuestions { get; set; }

        [Required(ErrorMessage = "Question text is required.")]
        [Display(Name = "Question Text")]
        public string QuestionText { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option A is required.")]
        [Display(Name = "Option A")]
        public string OptionA { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option B is required.")]
        [Display(Name = "Option B")]
        public string OptionB { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option C is required.")]
        [Display(Name = "Option C")]
        public string OptionC { get; set; } = string.Empty;

        [Required(ErrorMessage = "Option D is required.")]
        [Display(Name = "Option D")]
        public string OptionD { get; set; } = string.Empty;

        [Required(ErrorMessage = "Correct option is required.")]
        [RegularExpression("^[A-D]$", ErrorMessage = "Correct option must be A, B, C, or D.")]
        [Display(Name = "Correct Option (A, B, C, D)")]
        public string CorrectOption { get; set; } = string.Empty;

        public string? SelectedOption { get; set; } // For student answers
        public int QuestionId { get; set; }
        public int QuestionBankId { get; set; }

        public QuestionViewModel() { }

        public QuestionViewModel(Question question, int NumberOfQuestions)
        {
            //Id = question.Id;
            //QuestionBankId = question.QuestionBankId;
            //QuestionNumber = question.QuestionNumber;
            //this.NumberOfQuestions = NumberOfQuestions;
            //QuestionText = question.QuestionText;
            //OptionA = question.OptionA;
            //OptionB = question.OptionB;
            //OptionC = question.OptionC;
            //OptionD = question.OptionD;
            //CorrectOption = question.CorrectOption;
            //SelectedOption = question.SelectedOption.ToString();

            Id = question.Id;
            QuestionBankId = question.QuestionBankId; // Changed from TestId
            QuestionNumber = question.QuestionNumber;
            QuestionText = question.QuestionText;
            OptionA = question.OptionA;
            OptionB = question.OptionB;
            OptionC = question.OptionC;
            OptionD = question.OptionD;
            CorrectOption = question.CorrectOption;
            NumberOfQuestions = NumberOfQuestions;
        }
    }
}