using CBTApplication.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;




namespace CBTApplication.Models
{

    public class QuestionBank
    {
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        //public int? CourseId { get; set; }

        //[ForeignKey("CourseId")]
        //public Course? Course { get; set; }

        public ICollection<Question> Questions { get; set; } = new List<Question>();
        public ICollection<Test> Tests { get; set; } = new List<Test>();

        [Display(Name = "Date Created")]
        public DateTime DateCreated { get; set; }
    }

}