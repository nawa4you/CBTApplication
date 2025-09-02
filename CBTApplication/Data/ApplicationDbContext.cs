using CBTApplication.Models;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;
using System.Text;

namespace CBTApplication.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public DbSet<User> Users { get; set; }
        public DbSet<Test> Tests { get; set; }
        public DbSet<Question> Questions { get; set; }
        public DbSet<TestAttempt> TestAttempts { get; set; }
        public DbSet<StudentAnswer> StudentAnswers { get; set; }
        public DbSet<PasswordResetRequest> PasswordResetRequests { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Course> Courses { get; set; }
        public DbSet<QuestionBank> QuestionBanks { get; set; }
        public DbSet<TestDepartment> TestDepartments { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure many-to-many relationship between Test and Department
            modelBuilder.Entity<TestDepartment>()
                .HasKey(td => new { td.TestId, td.DepartmentId });

            modelBuilder.Entity<TestDepartment>()
                .HasOne(td => td.Test)
                .WithMany(t => t.TestDepartments)
                .HasForeignKey(td => td.TestId);

            modelBuilder.Entity<TestDepartment>()
                .HasOne(td => td.Department)
                .WithMany(d => d.TestDepartments)
                .HasForeignKey(td => td.DepartmentId);

            // Configure TestAttempt-User relationship
            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.User)
                .WithMany(u => u.TestAttempts)
                .HasForeignKey(ta => ta.UserId)
                .HasPrincipalKey(u => u.Email)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure Question-QuestionBank relationship
            modelBuilder.Entity<Question>()
                .HasOne(q => q.QuestionBank)
                .WithMany(qb => qb.Questions)
                .HasForeignKey(q => q.QuestionBankId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure TestAttempt-Test relationship
            modelBuilder.Entity<TestAttempt>()
                .Property(ta => ta.NumberOfQuestions)
                .IsRequired();

            modelBuilder.Entity<TestAttempt>()
                .HasOne(ta => ta.Test)
                .WithMany(t => t.TestAttempts)
                .HasForeignKey(ta => ta.TestId)
                .OnDelete(DeleteBehavior.Cascade);

            // Configure StudentAnswer relationships - FIXED CASCADE DELETE ISSUE
            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.TestAttempt)
                .WithMany(ta => ta.StudentAnswers)
                .HasForeignKey(sa => sa.TestAttemptId)
                .OnDelete(DeleteBehavior.Restrict); // Changed from Cascade to Restrict

            modelBuilder.Entity<StudentAnswer>()
                .HasOne(sa => sa.Question)
                .WithMany()
                .HasForeignKey(sa => sa.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<StudentAnswer>()
                .Property(sa => sa.OrderIndex)
                .IsRequired();

            // Configure User relationships
            modelBuilder.Entity<User>()
                .HasOne(u => u.Department)
                .WithMany()
                .HasForeignKey(u => u.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<User>()
                .HasOne(u => u.Course)
                .WithMany()
                .HasForeignKey(u => u.CourseId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure Test-QuestionBank relationship
            modelBuilder.Entity<Test>()
                .HasOne(t => t.QuestionBank)
                .WithMany(qb => qb.Tests)
                .HasForeignKey(t => t.QuestionBankId)
                .OnDelete(DeleteBehavior.Restrict);

            // Configure properties
            modelBuilder.Entity<TestAttempt>()
                .Property(ta => ta.CurrentQuestionIndex)
                .HasDefaultValue(0);

            // Seed initial data
            modelBuilder.Entity<Department>().HasData(
                new Department { Id = 1, DepartmentName = "Computer Science" },
                new Department { Id = 2, DepartmentName = "Electrical Engineering" },
                new Department { Id = 3, DepartmentName = "Business Administration" }
            );

            modelBuilder.Entity<Course>().HasData(
                new Course { Id = 1, CourseName = "CBT-101", DepartmentId = 1 },
                new Course { Id = 2, CourseName = "Data Structures", DepartmentId = 1 },
                new Course { Id = 3, CourseName = "Circuit Analysis", DepartmentId = 2 },
                new Course { Id = 4, CourseName = "Digital Electronics", DepartmentId = 2 },
                new Course { Id = 5, CourseName = "Business Fundamentals", DepartmentId = 3 },
                new Course { Id = 6, CourseName = "Marketing Principles", DepartmentId = 3 }
            );

            modelBuilder.Entity<QuestionBank>().HasData(
                new QuestionBank { Id = 1, Name = "Computer Science Fundamentals", DateCreated = new DateTime(2025, 10, 10) },
                new QuestionBank { Id = 2, Name = "Electrical Engineering Basics", DateCreated = new DateTime(2025, 10, 10) },
                new QuestionBank { Id = 3, Name = "Business Administration Essentials", DateCreated = new DateTime(2025, 10, 10) }
            );

            modelBuilder.Entity<User>().HasData(
                new User { Id = 1, Email = "daniel@gmail.com", PasswordHash = "$2a$11$y8HJdlnpn0MoWczzU1E5SeVRPFIp9TyAYefFHyrTw8dvIZMEvrlM2", Role = "Admin", Name = "Daniel Ezinna", PhoneNumber = "08011110001", Gender = "Male", Level = "N/A", UniqueId = "ADM002", DepartmentId = 1, CourseId = 1 },
                new User { Id = 2, Email = "yamal@gmail.com", PasswordHash = "$2a$11$eziRJeLZaPDFpn7xiYBXo.M58tOlzaomNDpI5ImdKppRAvbE.kkMu", Role = "User", Name = "Yamal", PhoneNumber = "08033483201", Gender = "Male", Level = "300", UniqueId = "2312938422", DepartmentId = 1, CourseId = 2 },
                new User { Id = 3, Email = "alice@example.com", PasswordHash = "$2a$11$eziRJeLZaPDFpn7xiYBXo.M58tOlzaomNDpI5ImdKppRAvbE.kkMu", Role = "User", Name = "Alice Smith", PhoneNumber = "08012345678", Gender = "Female", Level = "200", UniqueId = "2312938423", DepartmentId = 2, CourseId = 3 },
                new User { Id = 4, Email = "bob@example.com", PasswordHash = "$2a$11$eziRJeLZaPDFpn7xiYBXo.M58tOlzaomNDpI5ImdKppRAvbE.kkMu", Role = "User", Name = "Bob Johnson", PhoneNumber = "08087654321", Gender = "Male", Level = "400", UniqueId = "2312938424", DepartmentId = 3, CourseId = 5 }
            );

            modelBuilder.Entity<Test>().HasData(
                new Test { Id = 1, TestName = "Computer Science Fundamentals Test", DurationMinutes = 45, NumberOfQuestions = 5, IsActive = true, QuestionBankId = 1, DateCreated = new DateTime(2025, 10, 10) },
                new Test { Id = 2, TestName = "Electrical Engineering Basics Test", DurationMinutes = 45, NumberOfQuestions = 5, IsActive = true, QuestionBankId = 2, DateCreated = new DateTime(2025, 10, 10) },
                new Test { Id = 3, TestName = "Business Administration Essentials Test", DurationMinutes = 45, NumberOfQuestions = 5, IsActive = true, QuestionBankId = 3, DateCreated = new DateTime(2025, 10, 10) }
            );

            modelBuilder.Entity<Question>().HasData(
                new Question { Id = 1, QuestionBankId = 1, QuestionNumber = 1, QuestionText = "What does CPU stand for?", OptionA = "Central Processing Unit", OptionB = "Computer Processing Unit", OptionC = "Central Program Utility", OptionD = "Computer Program Unit", CorrectOption = "A" },
                new Question { Id = 2, QuestionBankId = 1, QuestionNumber = 2, QuestionText = "Which programming language is known as the 'mother of all languages'?", OptionA = "C", OptionB = "Java", OptionC = "Assembly", OptionD = "Fortran", CorrectOption = "D" },
                new Question { Id = 3, QuestionBankId = 1, QuestionNumber = 3, QuestionText = "What data structure uses LIFO (Last In, First Out) principle?", OptionA = "Queue", OptionB = "Stack", OptionC = "Linked List", OptionD = "Tree", CorrectOption = "B" },
                new Question { Id = 4, QuestionBankId = 1, QuestionNumber = 4, QuestionText = "Which of these is not a programming paradigm?", OptionA = "Object-Oriented", OptionB = "Functional", OptionC = "Procedural", OptionD = "Circular", CorrectOption = "D" },
                new Question { Id = 5, QuestionBankId = 1, QuestionNumber = 5, QuestionText = "What does HTML stand for?", OptionA = "HyperText Markup Language", OptionB = "HighText Machine Language", OptionC = "HyperTabular Markup Language", OptionD = "None of these", CorrectOption = "A" },
                new Question { Id = 6, QuestionBankId = 2, QuestionNumber = 1, QuestionText = "What is the unit of electrical resistance?", OptionA = "Volt", OptionB = "Ampere", OptionC = "Ohm", OptionD = "Watt", CorrectOption = "C" },
                new Question { Id = 7, QuestionBankId = 2, QuestionNumber = 2, QuestionText = "Which law states that the current through a conductor is proportional to the voltage?", OptionA = "Faraday's Law", OptionB = "Ohm's Law", OptionC = "Kirchhoff's Law", OptionD = "Coulomb's Law", CorrectOption = "B" },
                new Question { Id = 8, QuestionBankId = 2, QuestionNumber = 3, QuestionText = "What does AC stand for in electrical engineering?", OptionA = "Alternating Current", OptionB = "Active Current", OptionC = "Ampere Current", OptionD = "Absolute Current", CorrectOption = "A" },
                new Question { Id = 9, QuestionBankId = 2, QuestionNumber = 4, QuestionText = "Which component stores electrical energy in an electric field?", OptionA = "Resistor", OptionB = "Inductor", OptionC = "Transistor", OptionD = "Capacitor", CorrectOption = "D" },
                new Question { Id = 10, QuestionBankId = 2, QuestionNumber = 5, QuestionText = "What is the SI unit of electrical conductance?", OptionA = "Ohm", OptionB = "Siemens", OptionC = "Henry", OptionD = "Farad", CorrectOption = "B" },
                new Question { Id = 11, QuestionBankId = 3, QuestionNumber = 1, QuestionText = "What does SWOT stand for in business analysis?", OptionA = "Strengths, Weaknesses, Opportunities, Threats", OptionB = "Sales, Workforce, Operations, Technology", OptionC = "Strategy, Workflow, Organization, Tactics", OptionD = "None of these", CorrectOption = "A" },
                new Question { Id = 12, QuestionBankId = 3, QuestionNumber = 2, QuestionText = "Which of the following is not a type of business organization?", OptionA = "Sole Proprietorship", OptionB = "Partnership", OptionC = "Corporation", OptionD = "Individualism", CorrectOption = "D" },
                new Question { Id = 13, QuestionBankId = 3, QuestionNumber = 3, QuestionText = "What is the primary goal of a business?", OptionA = "Social Welfare", OptionB = "Profit Maximization", OptionC = "Employee Satisfaction", OptionD = "Customer Delight", CorrectOption = "B" },
                new Question { Id = 14, QuestionBankId = 3, QuestionNumber = 4, QuestionText = "Which management function involves setting objectives and determining a course of action?", OptionA = "Organizing", OptionB = "Leading", OptionC = "Planning", OptionD = "Controlling", CorrectOption = "C" },
                new Question { Id = 15, QuestionBankId = 3, QuestionNumber = 5, QuestionText = "What does ROI stand for in business?", OptionA = "Return on Investment", OptionB = "Rate of Interest", OptionC = "Return on Income", OptionD = "Rate of Inflation", CorrectOption = "A" }
            );

            modelBuilder.Entity<TestDepartment>().HasData(
                new TestDepartment { TestId = 1, DepartmentId = 1 },
                new TestDepartment { TestId = 2, DepartmentId = 2 },
                new TestDepartment { TestId = 3, DepartmentId = 3 }
            );
        }
    }
}