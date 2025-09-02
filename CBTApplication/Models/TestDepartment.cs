
using System.ComponentModel.DataAnnotations.Schema;

namespace CBTApplication.Models
{
    public class TestDepartment
    {
        public int TestId { get; set; }
        public Test Test { get; set; }

        public int DepartmentId { get; set; }
        public Department Department { get; set; }
    }
}