using Microsoft.AspNetCore.Mvc.Formatters;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CBTApplication.Models
{
    public class Course
    {
        [Key]
        public int Id { get; set; }
      

    





        [Required]
        [StringLength(100)]
        public string CourseName { get; set; } = string.Empty;



        public int DepartmentId { get; set; }

        // Navigation property for the related Department

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }
        // Collection of Tests associated with this Course
        public ICollection<Test>? Tests { get; set; } = new List<Test>();


    }
}
