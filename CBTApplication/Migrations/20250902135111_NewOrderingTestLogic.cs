using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace CBTApplication.Migrations
{
    /// <inheritdoc />
    public partial class NewOrderingTestLogic : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Departments",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DepartmentName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Departments", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PasswordResetRequests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserEmail = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RequestDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsProcessed = table.Column<bool>(type: "bit", nullable: false),
                    VerificationCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CodeExpiry = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PasswordResetRequests", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "QuestionBanks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionBanks", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Courses",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CourseName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Courses", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Courses_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Questions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    QuestionBankId = table.Column<int>(type: "int", nullable: false),
                    QuestionNumber = table.Column<int>(type: "int", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionA = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionB = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionC = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    OptionD = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CorrectOption = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Questions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Questions_QuestionBanks_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "QuestionBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Tests",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    NumberOfQuestions = table.Column<int>(type: "int", nullable: false),
                    QuestionBankId = table.Column<int>(type: "int", nullable: false),
                    DateCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CurrentQuestionIndex = table.Column<int>(type: "int", nullable: false),
                    CourseId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tests_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Tests_QuestionBanks_QuestionBankId",
                        column: x => x.QuestionBankId,
                        principalTable: "QuestionBanks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: true),
                    CourseId = table.Column<int>(type: "int", nullable: true),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Gender = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Level = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UniqueId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                    table.UniqueConstraint("AK_Users_Email", x => x.Email);
                    table.ForeignKey(
                        name: "FK_Users_Courses_CourseId",
                        column: x => x.CourseId,
                        principalTable: "Courses",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_Users_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestDepartments",
                columns: table => new
                {
                    TestId = table.Column<int>(type: "int", nullable: false),
                    DepartmentId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestDepartments", x => new { x.TestId, x.DepartmentId });
                    table.ForeignKey(
                        name: "FK_TestDepartments_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestDepartments_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TestAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<string>(type: "nvarchar(100)", nullable: false),
                    TestId = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    Score = table.Column<int>(type: "int", nullable: true),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    DateAttempted = table.Column<DateTime>(type: "datetime2", nullable: false),
                    NumberOfQuestions = table.Column<int>(type: "int", nullable: false),
                    CurrentQuestionIndex = table.Column<int>(type: "int", nullable: false, defaultValue: 0),
                    DepartmentId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Departments_DepartmentId",
                        column: x => x.DepartmentId,
                        principalTable: "Departments",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_TestAttempts_Tests_TestId",
                        column: x => x.TestId,
                        principalTable: "Tests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Email",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "QuestionOrderNumber",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOption = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_QuestionOrderNumber", x => x.Id);
                    table.ForeignKey(
                        name: "FK_QuestionOrderNumber_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_QuestionOrderNumber_TestAttempts_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalTable: "TestAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "StudentAnswers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TestAttemptId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    QuestionId = table.Column<int>(type: "int", nullable: false),
                    SelectedOption = table.Column<string>(type: "nvarchar(1)", maxLength: 1, nullable: true),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_Questions_QuestionId",
                        column: x => x.QuestionId,
                        principalTable: "Questions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_StudentAnswers_TestAttempts_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalTable: "TestAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Departments",
                columns: new[] { "Id", "DepartmentName" },
                values: new object[,]
                {
                    { 1, "Computer Science" },
                    { 2, "Electrical Engineering" },
                    { 3, "Business Administration" }
                });

            migrationBuilder.InsertData(
                table: "QuestionBanks",
                columns: new[] { "Id", "DateCreated", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Computer Science Fundamentals" },
                    { 2, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Electrical Engineering Basics" },
                    { 3, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Business Administration Essentials" }
                });

            migrationBuilder.InsertData(
                table: "Courses",
                columns: new[] { "Id", "CourseName", "DepartmentId" },
                values: new object[,]
                {
                    { 1, "CBT-101", 1 },
                    { 2, "Data Structures", 1 },
                    { 3, "Circuit Analysis", 2 },
                    { 4, "Digital Electronics", 2 },
                    { 5, "Business Fundamentals", 3 },
                    { 6, "Marketing Principles", 3 }
                });

            migrationBuilder.InsertData(
                table: "Questions",
                columns: new[] { "Id", "CorrectOption", "OptionA", "OptionB", "OptionC", "OptionD", "QuestionBankId", "QuestionNumber", "QuestionText" },
                values: new object[,]
                {
                    { 1, "A", "Central Processing Unit", "Computer Processing Unit", "Central Program Utility", "Computer Program Unit", 1, 1, "What does CPU stand for?" },
                    { 2, "D", "C", "Java", "Assembly", "Fortran", 1, 2, "Which programming language is known as the 'mother of all languages'?" },
                    { 3, "B", "Queue", "Stack", "Linked List", "Tree", 1, 3, "What data structure uses LIFO (Last In, First Out) principle?" },
                    { 4, "D", "Object-Oriented", "Functional", "Procedural", "Circular", 1, 4, "Which of these is not a programming paradigm?" },
                    { 5, "A", "HyperText Markup Language", "HighText Machine Language", "HyperTabular Markup Language", "None of these", 1, 5, "What does HTML stand for?" },
                    { 6, "C", "Volt", "Ampere", "Ohm", "Watt", 2, 1, "What is the unit of electrical resistance?" },
                    { 7, "B", "Faraday's Law", "Ohm's Law", "Kirchhoff's Law", "Coulomb's Law", 2, 2, "Which law states that the current through a conductor is proportional to the voltage?" },
                    { 8, "A", "Alternating Current", "Active Current", "Ampere Current", "Absolute Current", 2, 3, "What does AC stand for in electrical engineering?" },
                    { 9, "D", "Resistor", "Inductor", "Transistor", "Capacitor", 2, 4, "Which component stores electrical energy in an electric field?" },
                    { 10, "B", "Ohm", "Siemens", "Henry", "Farad", 2, 5, "What is the SI unit of electrical conductance?" },
                    { 11, "A", "Strengths, Weaknesses, Opportunities, Threats", "Sales, Workforce, Operations, Technology", "Strategy, Workflow, Organization, Tactics", "None of these", 3, 1, "What does SWOT stand for in business analysis?" },
                    { 12, "D", "Sole Proprietorship", "Partnership", "Corporation", "Individualism", 3, 2, "Which of the following is not a type of business organization?" },
                    { 13, "B", "Social Welfare", "Profit Maximization", "Employee Satisfaction", "Customer Delight", 3, 3, "What is the primary goal of a business?" },
                    { 14, "C", "Organizing", "Leading", "Planning", "Controlling", 3, 4, "Which management function involves setting objectives and determining a course of action?" },
                    { 15, "A", "Return on Investment", "Rate of Interest", "Return on Income", "Rate of Inflation", 3, 5, "What does ROI stand for in business?" }
                });

            migrationBuilder.InsertData(
                table: "Tests",
                columns: new[] { "Id", "CourseId", "CurrentQuestionIndex", "DateCreated", "DurationMinutes", "IsActive", "NumberOfQuestions", "QuestionBankId", "TestName" },
                values: new object[,]
                {
                    { 1, null, 0, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 45, true, 5, 1, "Computer Science Fundamentals Test" },
                    { 2, null, 0, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 45, true, 5, 2, "Electrical Engineering Basics Test" },
                    { 3, null, 0, new DateTime(2025, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 45, true, 5, 3, "Business Administration Essentials Test" }
                });

            migrationBuilder.InsertData(
                table: "TestDepartments",
                columns: new[] { "DepartmentId", "TestId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 2, 2 },
                    { 3, 3 }
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "CourseId", "DepartmentId", "Email", "Gender", "Level", "Name", "PasswordHash", "PhoneNumber", "Role", "UniqueId" },
                values: new object[,]
                {
                    { 1, 1, 1, "daniel@gmail.com", "Male", "N/A", "Daniel Ezinna", "$2a$11$y8HJdlnpn0MoWczzU1E5SeVRPFIp9TyAYefFHyrTw8dvIZMEvrlM2", "08011110001", "Admin", "ADM002" },
                    { 2, 2, 1, "yamal@gmail.com", "Male", "300", "Yamal", "$2a$11$eziRJeLZaPDFpn7xiYBXo.M58tOlzaomNDpI5ImdKppRAvbE.kkMu", "08033483201", "User", "2312938422" },
                    { 3, 3, 2, "alice@example.com", "Female", "200", "Alice Smith", "$2a$11$eziRJeLZaPDFpn7xiYBXo.M58tOlzaomNDpI5ImdKppRAvbE.kkMu", "08012345678", "User", "2312938423" },
                    { 4, 5, 3, "bob@example.com", "Male", "400", "Bob Johnson", "$2a$11$eziRJeLZaPDFpn7xiYBXo.M58tOlzaomNDpI5ImdKppRAvbE.kkMu", "08087654321", "User", "2312938424" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Courses_DepartmentId",
                table: "Courses",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOrderNumber_QuestionId",
                table: "QuestionOrderNumber",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_QuestionOrderNumber_TestAttemptId",
                table: "QuestionOrderNumber",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_Questions_QuestionBankId",
                table: "Questions",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_QuestionId",
                table: "StudentAnswers",
                column: "QuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentAnswers_TestAttemptId",
                table: "StudentAnswers",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_DepartmentId",
                table: "TestAttempts",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_TestId",
                table: "TestAttempts",
                column: "TestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_UserId",
                table: "TestAttempts",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_TestDepartments_DepartmentId",
                table: "TestDepartments",
                column: "DepartmentId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_CourseId",
                table: "Tests",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Tests_QuestionBankId",
                table: "Tests",
                column: "QuestionBankId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_CourseId",
                table: "Users",
                column: "CourseId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_DepartmentId",
                table: "Users",
                column: "DepartmentId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PasswordResetRequests");

            migrationBuilder.DropTable(
                name: "QuestionOrderNumber");

            migrationBuilder.DropTable(
                name: "StudentAnswers");

            migrationBuilder.DropTable(
                name: "TestDepartments");

            migrationBuilder.DropTable(
                name: "Questions");

            migrationBuilder.DropTable(
                name: "TestAttempts");

            migrationBuilder.DropTable(
                name: "Tests");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "QuestionBanks");

            migrationBuilder.DropTable(
                name: "Courses");

            migrationBuilder.DropTable(
                name: "Departments");
        }
    }
}
