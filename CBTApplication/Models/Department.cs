using System.ComponentModel.DataAnnotations;

namespace CBTApplication.Models
{
    public class Department
    {
        [Key]
        public int Id { get; set; }
    


        [Required]
        [StringLength(100)]
        public string DepartmentName { get; set; } = string.Empty;

        public ICollection<TestDepartment> TestDepartments { get; set;} = new List<TestDepartment>();



    }

}
