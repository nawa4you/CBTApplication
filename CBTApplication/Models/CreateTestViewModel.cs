
//using Microsoft.AspNetCore.Mvc.Rendering;
//using System.ComponentModel.DataAnnotations;

//namespace CBTApplication.Models
//{
//    public class CreateTestViewModel
//    {
//        [Required(ErrorMessage = "Test name is required.")]
//        [StringLength(100, ErrorMessage = "Test name cannot exceed 100 characters.")]
//        [Display(Name = "Test Name")]
//        public string TestName { get; set; } = string.Empty;

//        [Required(ErrorMessage = "Duration is required.")]
//        [Range(1, 240, ErrorMessage = "Duration must be between 1 and 240 minutes.")]
//        [Display(Name = "Duration (Minutes)")]
//        public int DurationMinutes { get; set; }

//        [Required(ErrorMessage = "Number of questions is required.")]
//        [Range(1, 100, ErrorMessage = "Number of questions must be between 1 and 100.")]
//        [Display(Name = "Number of Questions")]
//        public int NumberOfQuestions { get; set; }

//        [Required(ErrorMessage = "Question bank is required.")]
//        [Display(Name = "Question Bank")]
//        public int QuestionBankId { get; set; }

//        [Required(ErrorMessage = "At least one department is required.")]
//        [Display(Name = "Assigned Departments")]
//        public List<int> DepartmentIds { get; set; } = new List<int>();


//        public List<Department> AllDepartments { get; set; } = new List<Department>();

//        public int? CourseId { get; set; }




//        public virtual QuestionBank QuestionBank { get; set; }



//        public List<QuestionBank> AllQuestionBanks { get; set; } = new List<QuestionBank>();


//        // For dropdown lists
//        public SelectList? Courses { get; set; }
//        public SelectList? QuestionBanks { get; set; }
//    }
//}





using CBTApplication.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.ComponentModel.DataAnnotations;
namespace CBTApplication.Models
{
    public class CreateTestViewModel
    {
        [Required(ErrorMessage = "Test name is required.")]
        [StringLength(100, ErrorMessage = "Test name cannot exceed 100 characters.")]
        [Display(Name = "Test Name")]
        public string TestName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Duration is required.")]
        [Range(1, 240, ErrorMessage = "Duration must be between 1 and 240 minutes.")]
        [Display(Name = "Duration (Minutes)")]
        public int DurationMinutes { get; set; }

        [Required(ErrorMessage = "Number of questions is required.")]
        [Range(1, 100, ErrorMessage = "Number of questions must be between 1 and 100.")]
        [Display(Name = "Number of Questions")]
        public int NumberOfQuestions { get; set; }

        [Required(ErrorMessage = "Department is required.")]
        [Display(Name = "Department")]
        public int DepartmentId { get; set; }   

        [Required(ErrorMessage = "Question bank is required.")] // Make sure this is here
        [Display(Name = "Question Bank")]
        public int QuestionBankId { get; set; }


        [Required(ErrorMessage = "At least one department is required.")]
        [Display(Name = "Assigned Departments")]
        public List<int> DepartmentIds { get; set; } = new List<int>();



        [Required(ErrorMessage = "Select at least one department.")]
        //public List<int> DepartmentIds { get; set; } = new List<int>();
        //public SelectList Departments { get; set; }
        //public SelectList QuestionBanks { get; set; }



        // For dropdown lists
        public SelectList Departments { get; set; }
        public SelectList QuestionBanks { get; set; }

        public CreateTestViewModel()
        {
            Departments = new SelectList(new List<Department>(), "Id", "DepartmentName");
            QuestionBanks = new SelectList(new List<QuestionBank>(), "Id", "Name");
        }
    }
}