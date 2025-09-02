using System.ComponentModel.DataAnnotations.Schema;



namespace CBTApplication.Models
{

    public class Question
    {
        public int Id { get; set; }

        [ForeignKey("QuestionBankId")]
        public int QuestionBankId { get; set; }

        public QuestionBank QuestionBank { get; set; }

        public int QuestionNumber { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public string OptionA { get; set; } = string.Empty;
        public string OptionB { get; set; } = string.Empty;
        public string OptionC { get; set; } = string.Empty;
        public string OptionD { get; set; } = string.Empty;
        public string CorrectOption { get; set; } = string.Empty;
        //public virtual QuestionBank QuestionBank { get; set; }


    }
}