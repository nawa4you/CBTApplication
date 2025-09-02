Computer-Based Test (CBT) Application




Project Description
This is a robust, full-stack web application built on C# and ASP.NET Core designed to facilitate computer-based examinations and assessments. The application provides a secure and intuitive platform for administrators to create and manage tests, question banks, and user accounts, while offering a seamless testing experience for students. Its architecture is built for scalability and data integrity, leveraging Entity Framework Core for database management and a clear separation of concerns.

Key Features
1. User Management and Authentication
Secure Registration & Login: The system uses bcrypt hashing for storing passwords, ensuring user credentials are never stored in plain text.

Email as Unique Identifier: Each user is uniquely identified by their email address, serving as a principal key for relationships with other entities like TestAttempts.

Password Reset Functionality: A secure password reset process is implemented using a time-sensitive verification code.

2. Role-Based Access Control (RBAC)
The application implements a simple but effective RBAC model to control user permissions and access to different parts of the system.

Admin Role: Has full access to all features, including creating, editing, and deleting tests and questions. Admins can also manage user accounts and view comprehensive reports.

User Role: Can log in, view their assigned tests, take tests, and review their own results. Users have restricted access to administrative functionalities.

3. Test Management
Test Creation: Administrators can create new tests, specifying a unique name, duration, and the number of questions to be drawn from a specific question bank.

Active Status: Tests can be activated or deactivated, controlling their visibility and availability to students.

Secure URL Routes: Key administrative URLs are protected using ASP.NET Core's built-in authorization, ensuring that only users with the Admin role can access them.

4. Question Banks
Centralized Storage: Questions are stored in a central repository, allowing them to be reused across multiple tests.

Content Organization: Questions are grouped into specific banks, making it easy for administrators to manage and maintain content.

Randomized Questions: The system can randomly select a specified number of questions from a question bank to create a unique test attempt for each user.

5. Test Attempt Logic
Real-time Progress Tracking: Each test attempt saves the user's selected answers and current progress, which is managed by a TestAttempt entity.

Randomized Question Order: To prevent cheating, the order of questions is randomized for each test attempt and stored in the QuestionOrderNumbers table.

Result Recording: Upon completion, the score and answers are recorded in the StudentAnswers and TestAttempts tables, respectively.

Prerequisites
To get this project up and running, you'll need the following:

.NET 8.0 SDK or higher

SQL Server (Express or LocalDB is sufficient)

Visual Studio or Visual Studio Code

Getting Started
1. Clone the Repository
git clone https://github.com/nawa4you/CBTApplication/
cd CBTApplication

2. Configure the Database Connection
Open appsettings.json and update the DefaultConnection string to point to your SQL Server instance.

{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CBTApplicationDb;Trusted_Connection=True;MultipleActiveResultSets=true"
  }
}

3. Run Database Migrations
Use the Entity Framework Core tools to create your database schema.

# Add a new migration to create the database schema
dotnet ef migrations add InitialCreate

# Apply the migration to create the database and seed initial data
dotnet ef database update

4. Run the Application
dotnet run

The application will launch, and you can navigate to the specified URL in your web browser.

Database Schema
The core of the application's data model is built around the following tables.

Users: Stores user accounts with properties like Email, Name, Role, and a hashed PasswordHash.

Tests: Contains test definitions (TestName, DurationMinutes, NumberOfQuestions).

Questions: Stores question content and options.

QuestionBanks: Organizes Questions.

TestAttempts: Tracks individual attempts and their overall status.

QuestionOrderNumbers: Links TestAttempts to Questions in a specific order.

StudentAnswers: Records the selected option for each question within a TestAttempt.

The relationships between these tables are configured to maintain data integrity, with appropriate foreign key constraints to prevent orphaned records.

Contributing
Pull requests are welcome. For major changes, please open an issue first to discuss what you would like to change.

License
MIT
