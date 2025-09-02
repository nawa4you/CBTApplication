using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Collections.Generic;

namespace CBTApplication.Models
{
    public class User
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required]
        public string PasswordHash { get; set; } = string.Empty;

        [Required]
        [StringLength(50)]
        public string Role { get; set; } = "User";

        // New properties for department and course
        public int? DepartmentId { get; set; } // Foreign key to Department
        public int? CourseId { get; set; } // Foreign key to Course

        [ForeignKey("DepartmentId")]
        public Department? Department { get; set; }

        [ForeignKey("CourseId")]
        public Course? Course { get; set; }

        // Existing properties and navigation properties
        public string? PhoneNumber { get; set; }
        public string? Gender { get; set; }
        public string? Level { get; set; }
        public string? UniqueId { get; set; }
        public ICollection<TestAttempt> TestAttempts { get; set; } = new List<TestAttempt>();
        //public ICollection<PasswordResetRequest> PasswordResetRequests { get; set; } = new List<PasswordResetRequest>();
    }
}