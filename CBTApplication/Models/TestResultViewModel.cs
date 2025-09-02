//// Models/TestResultViewModel.cs

//using CBTApplication.Data;
//using System.Collections.Generic;
//using static CBTApplication.Controllers.HomeController;

//namespace CBTApplication.Models
//{
//    public class TestResultViewModel
//    {
//        public string? TestName { get; set; }


//        public int? NumberOfQuestions { get; set; }
//        public string? StudentName { get; set; }

//            public int TestAttemptId { get; set; }
        


//        public int? Score { get; set; }
//        public int TotalQuestions { get; set; }
//        public ICollection<StudentAnswer>? StudentAnswers { get; set; }

//        public List<ResultQuestionViewModel> Questions { get; set; } = new List<ResultQuestionViewModel>();





//    }
//}









using System.Collections.Generic;
using static CBTApplication.Controllers.HomeController;

namespace CBTApplication.Models
{
    public class TestResultViewModel
    {
        public string StudentName { get; set; } = string.Empty;
        public string TestName { get; set; } = string.Empty;
        public int Score { get; set; }
        public int NumberOfQuestions { get; set; }
        public List<ResultQuestionViewModel> Questions { get; set; } = new List<ResultQuestionViewModel>();

        public class QuestionResultModel
        {
            public string QuestionText { get; set; } = string.Empty;
            public string OptionA { get; set; } = string.Empty;
            public string OptionB { get; set; } = string.Empty;
            public string OptionC { get; set; } = string.Empty;
            public string OptionD { get; set; } = string.Empty;
            public string CorrectOption { get; set; } = string.Empty;
            public string SelectedOption { get; set; } = string.Empty;
            public bool IsCorrect => SelectedOption == CorrectOption;
        }
    }
}