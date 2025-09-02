using CBTApplication.Data;
using CBTApplication.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;
using System.Security.Claims;
using System.Text.Json;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory.Model;


namespace CBTApplication.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        // The Index action now acts as a routing hub for authenticated users.
        public IActionResult Index()
        {
            if (User.Identity?.IsAuthenticated == true)
            {
                if (User.IsInRole("Admin"))
                {
                    return RedirectToAction("AdminDashboard");
                }
                else if (User.IsInRole("User"))
                {
                    return RedirectToAction("UserDashboard");
                }
            }
            // For unauthenticated users, or if roles are not set, show the default homepage.
            return RedirectToAction("Login");
        }
        [HttpGet]
        [Authorize]
        public IActionResult ViewInfo()
        {
            var model = new ChangePasswordViewModel();


            return View(model);
        }


        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
        {
            var userEmail = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("Login");
            }

            if (!ModelState.IsValid)
            {
                return View("ViewInfo", model);
            }

            if (!BCrypt.Net.BCrypt.Verify(model.CurrentPassword, user.PasswordHash))
            {
                ModelState.AddModelError(nameof(model.CurrentPassword), "Invalid current password.");
                return View("ViewInfo", model);
            }

            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 11);

            user.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Password changed successfully!";
            Console.WriteLine("Successfully changed password");
            return RedirectToAction("ViewInfo");
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _context.Users
                    .Include(u => u.Department)
                    .FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null && BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                {
                    var claims = new List<Claim>
                    {
                        new Claim(ClaimTypes.NameIdentifier, user.Email),
                        new Claim(ClaimTypes.Email, user.Email),
                        new Claim(ClaimTypes.Name, user.Name ?? user.Email),
                        new Claim(ClaimTypes.Role, user.Role),
                        new Claim(ClaimTypes.MobilePhone, user.PhoneNumber),
                        new Claim(ClaimTypes.Gender, user.Gender),
                        //new Claim(ClaimTypes.UserData, user.Gender),
                        new Claim(ClaimTypes.UserData, user.Level),
                        new Claim(ClaimTypes.SerialNumber, user.UniqueId),
                        
                        new Claim("Department", user.Department?.DepartmentName?? string.Empty)
                    };
                    

                    // With the following correct Claim construction:
                    var claimsIdentity = new ClaimsIdentity(
                        claims, CookieAuthenticationDefaults.AuthenticationScheme);

                    var authProperties = new AuthenticationProperties
                    {


                        IsPersistent = model.RememberMe
                    };

                    await HttpContext.SignInAsync(
                        CookieAuthenticationDefaults.AuthenticationScheme,
                        new ClaimsPrincipal(claimsIdentity),
                        authProperties);

                    TempData["SuccessMessage"] = $"Welcome back, {user.Name}!";
                    return RedirectToAction("Index");
                }

                TempData["ErrorMessage"] = "User Not Found";
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            TempData["SuccessMessage"] = "You have been logged out successfully.";
            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult RegisterUser()
        {
            return View(new RegisterViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUser(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (existingUser != null)
                {
                    ModelState.AddModelError("Email", "A user with this email already exists.");
                    return View(model);
                }

                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password, 11);

                var newUser = new User
                {
                    Email = model.Email,
                    PasswordHash = hashedPassword,
                    Name = model.Name,
                    PhoneNumber = model.PhoneNumber,
                    Gender = model.Gender,
                    Level = model.Level,
                    Role = "User",
                  
                    //CourseId= model. ,
                    UniqueId = model.UniqueId,

                    //Department = model.Department


                };

                _context.Users.Add(newUser);
                await _context.SaveChangesAsync();

                // Automatically sign in the user after successful registration
                var claims = new List<Claim>
                {
                    new Claim(ClaimTypes.NameIdentifier, newUser.Email),
                    new Claim(ClaimTypes.Email, newUser.Email),
                    new Claim(ClaimTypes.Name, newUser.Name ?? newUser.Email),
                    new Claim(ClaimTypes.Role, newUser.Role)
                };
                var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, new ClaimsPrincipal(claimsIdentity));

                TempData["SuccessMessage"] = "Registration successful! You have been logged in automatically.";
                return RedirectToAction("UserDashboard"); // Redirect to the user's dashboard.
            }

            return View(model);
        }

        // NEW: Dedicated action for the user dashboard.
        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> UserDashboard()
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var user = await _context.Users
                .Include(u => u.Department)
                .FirstOrDefaultAsync(u => u.Email == userId);

            if (user == null)
            {
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                TempData["ErrorMessage"] = "User account not found. Please log in again.";
                return RedirectToAction("Login");
            }

            ViewBag.UserName = user.Name ?? "";

            // Get tests available to the user's department
            var availableTestIds = await _context.TestDepartments
                .Where(td => td.DepartmentId == user.DepartmentId)
                .Select(td => td.TestId)
                .ToListAsync();

            var viewModel = new UserDashboardViewModel
            {
                // Only show active tests for the user's department
                AvailableTests = await _context.Tests
                    .Include(t => t.QuestionBank)
                    .Where(t => t.IsActive && availableTestIds.Contains(t.Id))
                    .ToListAsync(),

                PastTestAttempts = await _context.TestAttempts
                    .Include(ta => ta.Test)
                    .Where(ta => ta.UserId == user.Email && ta.IsCompleted)
                    .ToListAsync()
            };

            return View(viewModel);
        }




        [Authorize (Roles = "Admin")]
        public async Task<IActionResult> AdminDashboard()
        {
            var testAttempts = await _context.TestAttempts
                .Include(ta => ta.Test)
                .Include(ta => ta.User)
                .Where(ta => ta.IsCompleted)
                .OrderByDescending(ta => ta.EndTime)
                .ToListAsync();

            var viewModel = new AdminDashboardViewModel 
            {
                CompletedTestAttempts = testAttempts
            };
            return View(viewModel);
        }


        //NEW password model



        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RequestPasswordReset([FromBody] PasswordResetRequestModel requestModel)
        {


            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();
            DateTime codeExpiry = DateTime.UtcNow.AddMinutes(5);

            var newRequest = new PasswordResetRequest
            {
                UserEmail = requestModel.Email,
                RequestDate = DateTime.UtcNow,
                IsProcessed = false,
                VerificationCode = verificationCode,
                CodeExpiry = codeExpiry
            };

            _context.PasswordResetRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password reset request initiated for {requestModel.Email}. Code: {verificationCode}, Expires: {codeExpiry}.");

            return Json(new { success = true, email = requestModel.Email, verificationCode = verificationCode });
        }

        public class PasswordResetRequestModel
        {
            public string Email { get; set; } = string.Empty;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult ForgotPassword()
        {
            return View(new ForgotPasswordViewModel());
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword(ForgotPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);

            if (user == null)
            {
                TempData["SuccessMessage"] = "If an account exists for that email address, a password reset request has been initiated.";
                return RedirectToAction("ForgotPasswordConfirmation");
            }

            var existingPendingRequests = await _context.PasswordResetRequests
                .Where(r => r.UserEmail == model.Email && !r.IsProcessed)
                .ToListAsync();
            foreach (var req in existingPendingRequests)
            {
                _context.PasswordResetRequests.Remove(req);
            }
            await _context.SaveChangesAsync();

            Random random = new Random();
            string verificationCode = random.Next(100000, 999999).ToString();
            DateTime codeExpiry = DateTime.UtcNow.AddMinutes(5);

            var newRequest = new PasswordResetRequest
            {
                UserEmail = model.Email,
                RequestDate = DateTime.UtcNow,
                IsProcessed = false,
                VerificationCode = verificationCode,
                CodeExpiry = codeExpiry
            };

            _context.PasswordResetRequests.Add(newRequest);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Password reset request initiated from ForgotPassword page for {model.Email}. Code: {verificationCode}, Expires: {codeExpiry}.");

            return RedirectToAction("VerifyCode", new { email = model.Email, code = verificationCode });
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult VerifyCode(string email, string code)
        {
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Invalid request. Please start the password reset process again.";
                return RedirectToAction("ForgotPassword");
            }

            var viewModel = new VerifyCodeViewModel
            {
                Email = email,
                GeneratedCodeForDisplay = code
            };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> VerifyCode(VerifyCodeViewModel model)
        {
            if (!ModelState.IsValid)
            {
                var pendingRequest = await _context.PasswordResetRequests
                    .FirstOrDefaultAsync(r => r.UserEmail == model.Email && !r.IsProcessed);
                if (pendingRequest != null)
                {
                    model.GeneratedCodeForDisplay = pendingRequest.VerificationCode;
                }
                return View(model);
            }

            var passwordResetRequest = await _context.PasswordResetRequests
                .FirstOrDefaultAsync(r => r.UserEmail == model.Email && !r.IsProcessed);

            if (passwordResetRequest == null ||
                passwordResetRequest.VerificationCode != model.EnteredCode ||
                passwordResetRequest.CodeExpiry <= DateTime.UtcNow)
            {
                ModelState.AddModelError(nameof(model.EnteredCode), "Invalid or expired verification code.");
                if (passwordResetRequest != null)
                {
                    model.GeneratedCodeForDisplay = passwordResetRequest.VerificationCode;
                }
                return View(model);
            }

            TempData["ResetEmail"] = model.Email;
            return RedirectToAction("SetNewPassword");
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult SetNewPassword()
        {
            string? email = TempData["ResetEmail"] as string;
            if (string.IsNullOrEmpty(email))
            {
                TempData["ErrorMessage"] = "Password reset session expired or invalid. Please start again.";
                return RedirectToAction("ForgotPassword");
            }

            var viewModel = new SetNewPasswordViewModel { Email = email };
            return View(viewModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetNewPassword(SetNewPasswordViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found for password reset.");
                return View(model);
            }

            var pendingRequest = await _context.PasswordResetRequests
                .FirstOrDefaultAsync(r => r.UserEmail == model.Email && !r.IsProcessed);

            if (pendingRequest == null || pendingRequest.CodeExpiry <= DateTime.UtcNow)
            {
                ModelState.AddModelError(string.Empty, "Password reset session expired or invalid. Please start again.");
                return View(model);
            }

            string newHashedPassword = BCrypt.Net.BCrypt.HashPassword(model.NewPassword, 11);
            user.PasswordHash = newHashedPassword;
            await _context.SaveChangesAsync();

            pendingRequest.IsProcessed = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Your password has been reset successfully!";
            return RedirectToAction("Login");
        }



        //end



        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageUsers()
        {
            var users = await _context.Users.OrderBy(u => u.Name).ToListAsync();
            return View(users);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ManageTests()
        {
            var tests = await _context.Tests
                .Include(t => t.QuestionBank)
                .Include(t => t.TestDepartments)
                    .ThenInclude(td => td.Department)
                .ToListAsync();

            return View(tests);
        }

        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CreateTest()
        //{
        //    ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name");
        //    ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name");
        //    return View();
        //}

        //[HttpPost]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> CreateTest(Test test)
        //{
        //    //    if (!ModelState.IsValid)
        //    //    {
        //    //        return View(model);
        //    //    }
        //    //    var newTest = new Test
        //    //    {
        //    //        TestName = model.TestName,
        //    //        DurationMinutes = model.DurationMinutes,
        //    //        NumberOfQuestions = model.NumberOfQuestions,
        //    //        IsActive = false
        //    //    };
        //    //    _context.Tests.Add(newTest);
        //    //    await _context.SaveChangesAsync();
        //    //    TempData["SuccessMessage"] = $"Test '{newTest.TestName}' created successfully. Now, add the questions.";


        //    //    return RedirectToAction("BuildTestQuestions", new { testId = newTest.Id, questionIndex = 1 });


        //    //if (ModelState.IsValid)
        //    //{
        //    //    _context.Add(test);
        //    //    await _context.SaveChangesAsync();
        //    //    TempData["SuccessMessage"] = "Test created successfully!";
        //    //    return RedirectToAction("AdminDashboard");
        //    //}

        //    //// If validation fails, re-populate dropdowns and return to the view
        //    //ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "Name", test.CourseId);
        //    //return View(test);


        //    // Populate the ViewBag with Courses, not Departments
        //    ViewBag.Courses = new SelectList(await _context.Courses.ToListAsync(), "Id", "CourseName");
        //    return View();

        //}




        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateTest()
        {
            var viewModel = new CreateTestViewModel
            {
                Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName"),
                QuestionBanks = new SelectList(await _context.QuestionBanks.ToListAsync(), "Id", "Name")
            };

            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateTest(CreateTestViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var test = new Test
                    {
                        TestName = model.TestName,
                        DurationMinutes = model.DurationMinutes,
                        NumberOfQuestions = model.NumberOfQuestions,
                        QuestionBankId = model.QuestionBankId,
                        DateCreated = DateTime.UtcNow,
                        IsActive = false
                    };

                    _context.Tests.Add(test);
                    await _context.SaveChangesAsync();

                    // Assign departments
                    if (model.DepartmentIds != null && model.DepartmentIds.Any())
                    {
                        foreach (var deptId in model.DepartmentIds)
                        {
                            _context.TestDepartments.Add(new TestDepartment
                            {
                                TestId = test.Id,
                                DepartmentId = deptId
                            });
                        }
                        await _context.SaveChangesAsync();
                    }

                    TempData["SuccessMessage"] = "Test created successfully!";
                    return RedirectToAction("ManageTests");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error creating test");
                    ModelState.AddModelError("", "An error occurred while creating the test. Please try again.");
                }
            }

            // Repopulate dropdowns if model state is invalid
            model.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName");
            model.QuestionBanks = new SelectList(await _context.QuestionBanks.ToListAsync(), "Id", "Name", model.QuestionBankId);

            return View(model);
        }





        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BuildTestQuestions(int testId, int questionIndex)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null || test.IsActive)
            {
                TempData["ErrorMessage"] = "Test not found or is already active.";
                return RedirectToAction("ManageTests");
            }
            if (questionIndex > test.NumberOfQuestions)
            {
                return RedirectToAction("FinalizeTest", new { testId = testId });
            }
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.QuestionBankId == testId && q.QuestionNumber == questionIndex);
            var viewModel = question != null ?
                new QuestionViewModel(question, test.NumberOfQuestions) :
                new QuestionViewModel { TestId = testId, QuestionNumber = questionIndex, NumberOfQuestions = test.NumberOfQuestions };
            ViewBag.TestName = test.TestName;
            ViewBag.QuestionCount = test.NumberOfQuestions;
            return View(viewModel);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> BuildTestQuestions(QuestionViewModel model)
        {
            var test = await _context.Tests.FirstOrDefaultAsync(t => t.Id == model.TestId);
            if (test == null || test.IsActive)
            {
                TempData["ErrorMessage"] = "Test not found or is already active.";
                return RedirectToAction("ManageTests");
            }
            if (!ModelState.IsValid)
            {
                ViewBag.TestName = test.TestName;
                ViewBag.QuestionCount = test.NumberOfQuestions;
                return View(model);
            }
            var question = await _context.Questions.FirstOrDefaultAsync(q => q.QuestionBankId == model.TestId && q.QuestionNumber == model.QuestionNumber);
            if (question == null)
            {
                question = new Question
                {
                    QuestionBankId = model.TestId,
                    QuestionNumber = model.QuestionNumber,
                    QuestionText = model.QuestionText,
                    OptionA = model.OptionA,
                    OptionB = model.OptionB,
                    OptionC = model.OptionC,
                    OptionD = model.OptionD,
                    CorrectOption = model.CorrectOption
                };
                _context.Questions.Add(question);
            }
            else
            {
                question.QuestionText = model.QuestionText;
                question.OptionA = model.OptionA;
                question.OptionB = model.OptionB;
                question.OptionC = model.OptionC;
                question.OptionD = model.OptionD;
                question.CorrectOption = model.CorrectOption;
                _context.Questions.Update(question);
            }
            await _context.SaveChangesAsync();
            if (model.QuestionNumber < test.NumberOfQuestions)
            {
                return RedirectToAction("BuildTestQuestions", new { testId = model.TestId, questionIndex = model.QuestionNumber + 1 });
            }
            else
            {
                TempData["SuccessMessage"] = $"All questions for test '{test.TestName}' have been saved. The test is now ready to be finalized.";
                return RedirectToAction("FinalizeTest", new { testId = model.TestId });
            }
        }

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> FinalizeTest(int testId)
        {
            var test = await _context.Tests.Include(t => t.QuestionBank).FirstOrDefaultAsync(t => t.Id == testId);
            if (test == null || test.IsActive || test.QuestionBank.Questions.Count < test.NumberOfQuestions)
            {
                TempData["ErrorMessage"] = "Test not found, already active, or is incomplete.";
                return RedirectToAction("ManageTests");
            }
            return View(test);
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ActivateTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("ManageTests");
            }

            test.IsActive = true;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Test '{test.TestName}' has been activated and is now available to users.";
            return RedirectToAction("ManageTests");
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeactivateTest(int id)
        {
            var test = await _context.Tests.FindAsync(id);
            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("ManageTests");
            }

            test.IsActive = false;
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = $"Test '{test.TestName}' has been deactivated.";
            return RedirectToAction("ManageTests");
        }


        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewTestQuestions(int testId)
        {
            var test = await _context.Tests
                .Include(t => t.QuestionBank)
                    .ThenInclude(qb => qb.Questions)
                .FirstOrDefaultAsync(t => t.Id == testId);

            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("ManageTests");
            }
            return View(test);
        }






        //[HttpGet]
        //[Authorize(Roles = "Admin")]
        //public async Task<IActionResult> DeleteTest(int id)
        //{
        //    var test = await _context.Tests.Include(t => t.QuestionBank).FirstOrDefaultAsync(t => t.Id == id);
        //    if (test == null)
        //    {
        //        TempData["ErrorMessage"] = "Test not found.";
        //        return RedirectToAction("ManageTests");
        //    }
        //    _context.Questions.RemoveRange(test.QuestionBank.Questions);
        //    _context.Tests.Remove(test);
        //    await _context.SaveChangesAsync();
        //    TempData["SuccessMessage"] = $"Test '{test.TestName}' and its questions have been deleted.";
        //    return RedirectToAction("ManageTests");
        //}

        [HttpGet]
        [Authorize]
        public IActionResult ViewTestResults()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }


        //public class CreateTestViewModel
        //{
        //    [Required(ErrorMessage = "Test name is required.")]
        //    [StringLength(100, ErrorMessage = "Test name cannot exceed 100 characters.")]
        //    [Display(Name = "Test Name")]
        //    public string TestName { get; set; } = string.Empty;
        //    [Required(ErrorMessage = "Duration is required.")]
        //    [Range(1, 240, ErrorMessage = "Duration must be between 1 and 240 minutes.")]
        //    [Display(Name = "Duration (Minutes)")]
        //    public int DurationMinutes { get; set; }
        //    [Required(ErrorMessage = "Number of questions is required.")]
        //    [Range(1, 100, ErrorMessage = "Number of questions must be between 1 and 100.")]
        //    [Display(Name = "Number of Questions")]
        //    public int NumberOfQuestions { get; set; }


        //    [Required(ErrorMessage = "Question bank is required.")]
        //    [Display(Name = "Question Bank")]
        //    public int QuestionBankId { get; set; }

        //    [Required(ErrorMessage = "At least one department is required.")]
        //    [Display(Name = "Assigned Departments")]
        //    public List<int> DepartmentIds { get; set; } = new List<int>();


        //    public List<Department> AllDepartments { get; set; } = new List<Department>();


        //    public List<QuestionBank> AllQuestionBanks { get; set; } = new List<QuestionBank>();
        //}
























        //take test



        //[Authorize]
        //public async Task<IActionResult> TestResults(int testAssignmentId)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    var attempt = await _context.TestAttempts
        //        .Where(ta => ta.UserId == email)
        //        .Include(ta => ta.Test)
        //        .Include(ta => ta.StudentAnswers)
        //        .ThenInclude(sa => sa.Question)
        //        .FirstOrDefaultAsync(ta => ta.Id == testAssignmentId);

        //    if (attempt == null)
        //    {
        //        TempData["ErrorMessage"] = "Test results not found.";
        //        return RedirectToAction("UserDashboard");
        //    }

        //    var correctAnswers = attempt.StudentAnswers.Count(sa => sa.SelectedOption == sa.Question.CorrectOption);
        //    attempt.Score = correctAnswers;
        //    await _context.SaveChangesAsync();








        //    var viewModel = new TestResultViewModel
        //    {
        //        TestName = attempt.Test.TestName,
        //        Score = attempt.Score,
        //        TotalQuestions = attempt.Test.NumberOfQuestions,
        //        StudentAnswers = attempt.StudentAnswers
        //    };

        //    return View(viewModel);
        //}









        //real

        //[HttpPost]
        //public async Task<IActionResult> TakeTest(TakeTestViewModel model)
        //{
        //    var email = User.FindFirstValue(ClaimTypes.Email);
        //    var attempt = await _context.TestAttempts
        //        .Include(ta => ta.StudentAnswers)
        //        .Include(ta => ta.Test)
        //            .ThenInclude(t => t.Questions)
        //        .FirstOrDefaultAsync(ta => ta.Id == model.TestAssignmentId && ta.UserId == email);

        //    if (attempt == null || attempt.IsCompleted)
        //    {
        //        TempData["ErrorMessage"] = "Test attempt not found or already completed.";
        //        return RedirectToAction("UserDashboard");
        //    }

        //    // Update remaining time
        //    attempt.TimeRemainingSeconds = model.TimeRemainingSeconds;

        //    // Process all submitted answers in a single loop
        //    foreach (var questionModel in model.Questions)
        //    {
        //        if (!string.IsNullOrEmpty(questionModel.SelectedOption))
        //        {
        //            var question = attempt.Test.Questions.FirstOrDefault(q => q.Id == questionModel.QuestionId);
        //            if (question != null)
        //            {
        //                var answer = attempt.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == question.Id);
        //                bool isCorrect = questionModel.SelectedOption == question.CorrectOption;

        //                if (answer == null)
        //                {
        //                    answer = new StudentAnswer
        //                    {
        //                        TestAttemptId = attempt.Id,
        //                        QuestionId = question.Id,
        //                        SelectedOption = questionModel.SelectedOption,
        //                        IsCorrect = isCorrect
        //                    };
        //                    _context.StudentAnswers.Add(answer);
        //                }
        //                else
        //                {
        //                    answer.SelectedOption = questionModel.SelectedOption;
        //                    answer.IsCorrect = isCorrect;
        //                }
        //            }
        //        }
        //    }

        //    // Handle navigation or submission
        //    if (model.Action == "submit" || attempt.TimeRemainingSeconds <= 0)
        //    {
        //        attempt.IsCompleted = true;
        //        attempt.EndTime = DateTime.UtcNow;
        //        attempt.Score = attempt.StudentAnswers.Count(sa =>
        //            sa.SelectedOption == attempt.Test.Questions
        //                .FirstOrDefault(q => q.Id == sa.QuestionId)?.CorrectOption);

        //        await _context.SaveChangesAsync();
        //        return RedirectToAction("AdminViewTestResults", new { TestAttemptId = attempt.Id });
        //    }

        //    // Update question index based on navigation
        //    var totalQuestions = attempt.Test.Questions.Count;
        //    if (model.Action == "next" && attempt.CurrentQuestionIndex < totalQuestions - 1)
        //    {
        //        attempt.CurrentQuestionIndex++;
        //    }
        //    else if (model.Action == "previous" && attempt.CurrentQuestionIndex > 0)
        //    {
        //        attempt.CurrentQuestionIndex--;
        //    }

        //    await _context.SaveChangesAsync();

        //    // Redirect to GET action to prevent form resubmission
        //    return RedirectToAction("TakeTest", new { testAssignmentId = attempt.Id });
        //}




        [HttpPost]
        [Authorize(Roles = "User")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TakeTest(TakeTestViewModel viewModel)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var testAttempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                .FirstOrDefaultAsync(ta => ta.Id == viewModel.TestAttemptId && ta.UserId == userEmail);

            if (testAttempt == null || testAttempt.IsCompleted)
            {
                TempData["ErrorMessage"] = "Test attempt not found or already completed.";
                return RedirectToAction("UserDashboard");
            }

            if (testAttempt.EndTime <= DateTime.UtcNow)
            {
                testAttempt.IsCompleted = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("SubmitTest", new { testAttemptId = testAttempt.Id });
            }

            // Get the current StudentAnswer based on OrderIndex
            var currentAttemptQuestion = await _context.StudentAnswers
                .FirstOrDefaultAsync(sa => sa.TestAttemptId == testAttempt.Id && sa.OrderIndex == testAttempt.CurrentQuestionIndex + 1);

            if (currentAttemptQuestion != null)
            {
                currentAttemptQuestion.SelectedOption = viewModel.SelectedOption;
            }

            switch (viewModel.Action)
            {
                case "next":
                    if (testAttempt.CurrentQuestionIndex + 1 < testAttempt.NumberOfQuestions)
                    {
                        testAttempt.CurrentQuestionIndex++;
                    }
                    break;
                case "previous":
                    if (testAttempt.CurrentQuestionIndex > 0)
                    {
                        testAttempt.CurrentQuestionIndex--;
                    }
                    break;
                case "submit":
                    await _context.SaveChangesAsync();
                    return RedirectToAction("SubmitTest", new { testAttemptId = testAttempt.Id });
            }

            await _context.SaveChangesAsync();
            return RedirectToAction("TakeTest", new { testAttemptId = testAttempt.Id });
        }


        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> SubmitTest(Guid testAttemptId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var testAttempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                .FirstOrDefaultAsync(ta => ta.Id == testAttemptId && ta.UserId == userEmail);

            if (testAttempt == null || testAttempt.IsCompleted)
            {
                TempData["ErrorMessage"] = "Test attempt not found or already completed.";
                return RedirectToAction("UserDashboard");
            }

            if (testAttempt.EndTime <= DateTime.UtcNow)
            {
                testAttempt.IsCompleted = true;
                await _context.SaveChangesAsync();
                TempData["Message"] = "Test time expired and has been submitted.";
                return RedirectToAction("UserDashboard");
            }

            var attemptQuestions = await _context.StudentAnswers
                .Include(sa => sa.Question)
                .Where(sa => sa.TestAttemptId == testAttempt.Id)
                .ToListAsync();

            int score = 0;
            foreach (var sa in attemptQuestions)
            {
                if (sa.SelectedOption != null)
                {
                    sa.IsCorrect = sa.SelectedOption == sa.Question.CorrectOption;
                    if (sa.IsCorrect) score++;
                }
            }

            testAttempt.Score = score;
            testAttempt.IsCompleted = true;
            testAttempt.EndTime = DateTime.UtcNow; // Set end time to now if submitted early

            await _context.SaveChangesAsync();
            TempData["Message"] = "Test submitted successfully.";
            return RedirectToAction("UserDashboard");
        }




















        //get


        //auth for user only before prodcution

        [HttpGet]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> TakeTest(Guid testAttemptId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);

            var testAttempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                    .ThenInclude(t => t.QuestionBank)
                        .ThenInclude(qb => qb.Questions)
                .FirstOrDefaultAsync(ta => ta.Id == testAttemptId && ta.UserId == userEmail);

            if (testAttempt == null || testAttempt.IsCompleted)
            {
                TempData["ErrorMessage"] = "Test attempt not found or already completed.";
                return RedirectToAction("UserDashboard");
            }

            // Calculate remaining time server-side
            var timeRemainingSeconds = (int)(testAttempt.EndTime.Value - DateTime.UtcNow).TotalSeconds;
            if (timeRemainingSeconds < 0)
            {
                testAttempt.IsCompleted = true;
                await _context.SaveChangesAsync();
                return RedirectToAction("SubmitTest", new { testAttemptId = testAttempt.Id });
            }

            // Get the ordered questions from StudentAnswers
            var attemptQuestions = await _context.StudentAnswers
                .Where(sa => sa.TestAttemptId == testAttemptId)
                .OrderBy(sa => sa.OrderIndex)
                .ToListAsync();

            if (!attemptQuestions.Any() || testAttempt.CurrentQuestionIndex >= attemptQuestions.Count)
            {
                TempData["ErrorMessage"] = "Invalid question index or no questions assigned.";
                return RedirectToAction("UserDashboard");
            }

            var currentAttemptQuestion = attemptQuestions[testAttempt.CurrentQuestionIndex];
            var currentQuestion = testAttempt.Test.QuestionBank.Questions
                .FirstOrDefault(q => q.Id == currentAttemptQuestion.QuestionId);

            if (currentQuestion == null)
            {
                TempData["ErrorMessage"] = "Question not found.";
                return RedirectToAction("UserDashboard");
            }

            var viewModel = new TakeTestViewModel
            {
                TestId = testAttempt.Test.Id,
                TestName = testAttempt.Test.TestName,
                NumberOfQuestions = testAttempt.NumberOfQuestions,
                DurationMinutes = testAttempt.Test.DurationMinutes,
                EndTime = testAttempt.EndTime,
                TestAttemptId = testAttempt.Id,
                TimeRemainingSeconds = timeRemainingSeconds,
                CurrentQuestionIndex = testAttempt.CurrentQuestionIndex,
                Questions = new List<QuestionViewModel>
        {
            new QuestionViewModel
            {
                QuestionId = currentQuestion.Id,
                QuestionText = currentQuestion.QuestionText,
                OptionA = currentQuestion.OptionA,
                OptionB = currentQuestion.OptionB,
                OptionC = currentQuestion.OptionC,
                OptionD = currentQuestion.OptionD,
                SelectedOption = currentAttemptQuestion.SelectedOption
            }
        }
            };

            return View(viewModel);
        }



        //end





























        //start test
        //[Authorize(Roles = "User")]
        //public async Task<IActionResult> StartTest(int testId)
        //{
        //    var test = await _context.Tests
        //        .Include(t => t.QuestionBank)
        //        .ThenInclude(qb => qb.Questions)
        //        .FirstOrDefaultAsync(t => t.Id == testId && t.IsActive);

        //    if (test == null)
        //    {
        //        TempData["ErrorMessage"] = "Test not found.";
        //        return RedirectToAction("UserDashboard");
        //    }

        //    var userEmail = User.FindFirstValue(ClaimTypes.Email);

        //    // Check if user already has an active attempt for this test
        //    var existingAttempt = await _context.TestAttempts
        //        .FirstOrDefaultAsync(ta => ta.TestId == testId && ta.UserId == userEmail && !ta.IsCompleted);

        //    if (existingAttempt != null)
        //    {
        //        return RedirectToAction("TakeTest", new { testAttemptId = existingAttempt.Id });
        //    }

        //    // Randomly select questions from the question bank
        //    var randomQuestions = test.QuestionBank.Questions
        //        .OrderBy(q => Guid.NewGuid())
        //        .Take(test.NumberOfQuestions)
        //        .ToList();

        //    // Serialize the question IDs to store them in the test attempt
        //    var questionIds = randomQuestions.Select(q => q.Id).ToList();
        //    var assignedQuestionsJson = JsonSerializer.Serialize(questionIds);

        //    // Create a new TestAttempt record to track the user's progress
        //    var testAttempt = new TestAttempt
        //    {
        //        UserId = userEmail,
        //        TestId = test.Id,
        //        StartTime = DateTime.UtcNow,
        //        IsCompleted = false,
        //        AssignedQuestionOrderJson = assignedQuestionsJson,
        //        TimeRemainingSeconds = test.DurationMinutes * 60,
        //        CurrentQuestionIndex = 0,
        //        NumberOfQuestions = test.NumberOfQuestions,
        //        DateAttempted = DateTime.UtcNow
        //    };

        //    _context.TestAttempts.Add(testAttempt);
        //    await _context.SaveChangesAsync();

        //    return RedirectToAction("TakeTest", new { testAttemptId = testAttempt.Id });
        //}



        [Authorize(Roles = "User")]
        public async Task<IActionResult> StartTest(int testId)
        {
            var userEmail = User.FindFirstValue(ClaimTypes.Email);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);

            if (user == null)
            {
                TempData["ErrorMessage"] = "User not found.";
                return RedirectToAction("UserDashboard");
            }

            var test = await _context.Tests
                .Include(t => t.QuestionBank)
                .ThenInclude(qb => qb.Questions)
                .FirstOrDefaultAsync(t => t.Id == testId && t.IsActive);

            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("UserDashboard");
            }

            var hasAccess = await _context.TestDepartments
                .AnyAsync(td => td.TestId == testId && td.DepartmentId == user.DepartmentId);

            if (!hasAccess)
            {
                TempData["ErrorMessage"] = "You do not have access to this test.";
                return RedirectToAction("UserDashboard");
            }

            var existingAttempt = await _context.TestAttempts
                .FirstOrDefaultAsync(ta => ta.TestId == testId && ta.UserId == userEmail && !ta.IsCompleted);

            if (existingAttempt != null)
            {
                return RedirectToAction("TakeTest", new { testAttemptId = existingAttempt.Id });
            }

            var randomQuestions = test.QuestionBank.Questions
                .OrderBy(q => Guid.NewGuid())
                .Take(test.NumberOfQuestions)
                .OrderBy(q => Guid.NewGuid())
                .ToList();

            var testAttempt = new TestAttempt
            {
                UserId = userEmail,
                TestId = test.Id,
                StartTime = DateTime.UtcNow,
                EndTime = DateTime.UtcNow.AddMinutes(test.DurationMinutes),
                IsCompleted = false,
                CurrentQuestionIndex = 0,
                NumberOfQuestions = test.NumberOfQuestions,
                DateAttempted = DateTime.UtcNow
            };

            _context.TestAttempts.Add(testAttempt);
            await _context.SaveChangesAsync();

            // Create StudentAnswer records with OrderIndex starting from 1
            for (int index = 0; index < randomQuestions.Count; index++)
            {
                var studentAnswer = new StudentAnswer
                {
                    TestAttemptId = testAttempt.Id,
                    OrderIndex = index + 1, // Start from 1
                    QuestionId = randomQuestions[index].Id,
                    SelectedOption = null,
                    IsCorrect = false
                };
                _context.StudentAnswers.Add(studentAnswer);
            }
            await _context.SaveChangesAsync();

            return RedirectToAction("TakeTest", new { testAttemptId = testAttempt.Id });
        }




        //int QuestNum = 1;
        //foreach (var item in questionIds)
        //{ 
        //    var studentAnser = new StudentAnswer
        //    {
        //        TestAttemptId = testAttempt.Id,
        //        QuestionId = item,
        //        //QestuinNum = QuestNum
        //    };
        //    QuestNum++;
        //}
        //await _context.SaveChangesAsync();








































        //edit test action

        // GET: EditTest
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> EditTest(int id)
        {
            var test = await _context.Tests
                .Include(t => t.TestDepartments)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("ManageTests");
            }

            var viewModel = new CreateTestViewModel
            {
                TestName = test.TestName,
                DurationMinutes = test.DurationMinutes,
                NumberOfQuestions = test.NumberOfQuestions,
                QuestionBankId = test.QuestionBankId,
                DepartmentIds = test.TestDepartments.Select(td => td.DepartmentId).ToList(),
                Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName"),
                QuestionBanks = new SelectList(await _context.QuestionBanks.ToListAsync(), "Id", "Name", test.QuestionBankId)
            };

            return View(viewModel);
        }

        // POST: EditTest
        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditTest(int id, CreateTestViewModel model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var test = await _context.Tests
                        .Include(t => t.TestDepartments)
                        .FirstOrDefaultAsync(t => t.Id == id);

                    if (test == null)
                    {
                        TempData["ErrorMessage"] = "Test not found.";
                        return RedirectToAction("ManageTests");
                    }

                    test.TestName = model.TestName;
                    test.DurationMinutes = model.DurationMinutes;
                    test.NumberOfQuestions = model.NumberOfQuestions;
                    test.QuestionBankId = model.QuestionBankId;

                    // Update department assignments
                    var currentDepartmentIds = test.TestDepartments.Select(td => td.DepartmentId).ToList();
                    var departmentsToRemove = currentDepartmentIds.Except(model.DepartmentIds);
                    var departmentsToAdd = model.DepartmentIds.Except(currentDepartmentIds);

                    // Remove departments that are no longer selected
                    foreach (var departmentId in departmentsToRemove)
                    {
                        var testDepartment = test.TestDepartments.FirstOrDefault(td => td.DepartmentId == departmentId);
                        if (testDepartment != null)
                        {
                            _context.TestDepartments.Remove(testDepartment);
                        }
                    }

                    // Add new departments
                    foreach (var departmentId in departmentsToAdd)
                    {
                        var testDepartment = new TestDepartment
                        {
                            TestId = test.Id,
                            DepartmentId = departmentId
                        };
                        _context.TestDepartments.Add(testDepartment);
                    }

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Test updated successfully!";
                    return RedirectToAction("ManageTests");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error updating test");
                    ModelState.AddModelError("", "An error occurred while updating the test. Please try again.");
                }
            }

            // If we got here, something failed; redisplay form
            model.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "DepartmentName");
            model.QuestionBanks = new SelectList(await _context.QuestionBanks.ToListAsync(), "Id", "Name", model.QuestionBankId);

            return View(model);
        }






































        //view result

        public class AdminViewTestResultsViewModel
        {
            public int TestAttemptId { get; set; }
            public string StudentName { get; set; } = string.Empty;
            public string TestName { get; set; } = string.Empty;
            public int Score { get; set; }
            public int NumberOfQuestions { get; set; }
            public List<ResultQuestionViewModel> Questions { get; set; } = new List<ResultQuestionViewModel>();


        }


        public async Task<IActionResult> AdminViewTestResults(Guid attemptId)
        {
            var attempt = await _context.TestAttempts
                .Include(ta => ta.Test)
                    .ThenInclude(t => t.QuestionBank)
                        .ThenInclude(qb => qb.Questions)
                .Include(ta => ta.StudentAnswers)
                .Include(ta => ta.User)
                .FirstOrDefaultAsync(ta => ta.Id == attemptId);

            if (attempt == null)
            {
                TempData["ErrorMessage"] = "Test results not found for this attempt.";
                return RedirectToAction("Index");
            }

            // Get the question IDs from StudentAnswers in order
            var questionIds = await _context.StudentAnswers
                .Where(sa => sa.TestAttemptId == attempt.Id)
                .OrderBy(sa => sa.OrderIndex)
                .Select(sa => sa.QuestionId)
                .ToListAsync();

            // Build a list of ResultQuestionViewModel for all questions in the test
            var questions = new List<ResultQuestionViewModel>();

            foreach (var questionId in questionIds)
            {
                var question = attempt.Test.QuestionBank.Questions.FirstOrDefault(q => q.Id == questionId);
                if (question != null)
                {
                    var answer = attempt.StudentAnswers.FirstOrDefault(sa => sa.QuestionId == questionId);

                    questions.Add(new ResultQuestionViewModel
                    {
                        QuestionId = question.Id,
                        QuestionText = question.QuestionText,
                        OptionA = question.OptionA,
                        OptionB = question.OptionB,
                        OptionC = question.OptionC,
                        OptionD = question.OptionD,
                        CorrectOption = question.CorrectOption,
                        SelectedOption = answer?.SelectedOption
                    });
                }
            }

            var viewModel = new TestResultViewModel
            {
                StudentName = attempt.User?.Name ?? attempt.UserId,
                TestName = attempt.Test.TestName,
                Score = attempt.Score ?? 0,
                NumberOfQuestions = attempt.NumberOfQuestions,
                Questions = questions
            };

            return View(viewModel);
        }





        public class AdminDashboardViewModel
        {
            public List<TestAttempt> CompletedTestAttempts { get; set; } = new List<TestAttempt>();

            //public virtual ICollection<TestDepartment> TestDepartments { get; set; } = new List<TestDepartment>();

        }






        public class ViewTestResultsViewModel
        {
            public int TestAttemptId { get; set; }
            public Test? TestName { get; set; }
            public int Score { get; set; }
            public int NumberOfQuestions { get; set; }
            public List<ResultQuestionViewModel> Questions { get; set; } = new List<ResultQuestionViewModel>();
        }

        public class ResultQuestionViewModel
        {

            public string QuestionText { get; set; } = string.Empty;
            public int QuestionId { get; set; }
            public string OptionA { get; set; } = string.Empty;
            public string OptionB { get; set; } = string.Empty;
            public string OptionC { get; set; } = string.Empty;
            public string OptionD { get; set; } = string.Empty;
            public string CorrectOption { get; set; } = string.Empty;
            public string? SelectedOption { get; set; }
            public bool IsCorrect => SelectedOption == CorrectOption;
        }

        //end






        //end

        public class RegisterViewModel
        {
            [Required(ErrorMessage = "Name is required.")]
            [StringLength(100)]
            [Display(Name = "Full Name")]
            public string Name { get; set; } = string.Empty;
            [Required(ErrorMessage = "Email is required.")]
            [EmailAddress]
            [Display(Name = "Email Address")]
            public string Email { get; set; } = string.Empty;
            [Required(ErrorMessage = "Password is required.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]


            public string Password { get; set; } = string.Empty;
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; } = string.Empty;
            [Display(Name = "Phone Number")]
            public string PhoneNumber { get; set; } = string.Empty;
            [Display(Name = "Gender")]
            public string Gender { get; set; } = string.Empty;
            [Display(Name = "Level")]
            public string Level { get; set; } = string.Empty;

            [Required(ErrorMessage = "Your matriculation is required.")]
            [Display(Name = "Matriculatoin Number")]
   
            
            
            public string UniqueId { get; set; } = string.Empty;
            [Required(ErrorMessage = "Your Department Name.")]
            [Display(Name = "Department")]
            public string Department { get; set; } = string.Empty;
        }

        public class UserDashboardViewModel
        {
            public List<Test> AvailableTests { get; set; } = new List<Test>();
            public List<TestAttempt> PastTestAttempts { get; set; } = new List<TestAttempt>();

            public List<TestAttempt> CompletedTestAttempts { get; set; } = new List<TestAttempt>();


            //public int CurrentQuestionIndex { get; set; } 
        }


        public class ResetPasswordViewModel
        {
            [Required(ErrorMessage = "New password is required.")]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at most {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "New Password")]
            public string NewPassword { get; set; } = string.Empty;
            [DataType(DataType.Password)]
            [Display(Name = "Confirm New Password")]
            [Compare("NewPassword", ErrorMessage = "The new password and confirmation password do not match.")]
            public string ConfirmNewPassword { get; set; } = string.Empty;
            [Required]
            public string Token { get; set; } = string.Empty;
        }

        public class ErrorViewModel
        {
            public string? RequestId { get; set; }
            public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);
        }

















        //create a new course

        [HttpGet]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> CreateCourse()
        {
            // Retrieve all departments from the database to populate a dropdown
            var departments = await _context.Departments.ToListAsync();
            ViewBag.Departments = new SelectList(departments, "Id", "Name");

            return View();
        }







        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateCourse(Course course)
        {
            if (ModelState.IsValid)
            {
                _context.Add(course);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Course created successfully!";
                return RedirectToAction("AdminDashboard");
            }

            // If we got this far, something failed, redisplay form with errors
            ViewBag.Departments = new SelectList(await _context.Departments.ToListAsync(), "Id", "Name", course.DepartmentId);
            return View(course);
        }
















        //admin adding question to the pool control

        // Add this new ViewModel class inside your HomeController
        //public class AddQuestionViewModel
        //{
        //    public int TestId { get; set; }

        //    //[Required]
        //    [Display(Name = "Test Name")]
        //    public string TestName { get; set; } = string.Empty;

        //    public List<Question> Questions { get; set; } = new List<Question>();



        //    [Required(ErrorMessage = "Question text is required.")]
        //    public string QuestionText { get; set; } = string.Empty;

        //    [Required(ErrorMessage = "Option A is required.")]
        //    public string OptionA { get; set; } = string.Empty;

        //    [Required(ErrorMessage = "Option B is required.")]
        //    public string OptionB { get; set; } = string.Empty;

        //    [Required(ErrorMessage = "Option C is required.")]
        //    public string OptionC { get; set; } = string.Empty;

        //    [Required(ErrorMessage = "Option D is required.")]
        //    public string OptionD { get; set; } = string.Empty;

        //    [Required(ErrorMessage = "Correct option is required.")]
        //    public string CorrectOption { get; set; } = string.Empty;
        //}

        // Add these two actions to your HomeController
        [Authorize(Roles = "Admin")]
        [HttpGet]
        public async Task<IActionResult> AddQuestion(int testId)
        {
            var test = await _context.Tests.FindAsync(testId);
            if (test == null)
            {
                return NotFound();
            }

            var questions = await _context.Questions.Where(q => q.QuestionBankId == testId).ToListAsync();
            var viewModel = new AddQuestionViewModel
            {
                TestId = test.Id,
                TestName = test.TestName,
                Questions = questions
            };

            return View(viewModel);
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestion(AddQuestionViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var question = new Question
                {
                    QuestionBankId = model.TestId,
                    QuestionText = Request.Form["Questions[0].QuestionText"],
                    OptionA = Request.Form["Questions[0].OptionA"],
                    OptionB = Request.Form["Questions[0].OptionB"],
                    OptionC = Request.Form["Questions[0].OptionC"],
                    OptionD = Request.Form["Questions[0].OptionD"],
                    CorrectOption = Request.Form["Questions[0].CorrectOption"],
                    QuestionNumber = _context.Questions.Count(q => q.QuestionBankId == model.TestId) + 1
                };

                _context.Questions.Add(question);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Question added successfully!";
                return RedirectToAction("AddQuestion", new { testId = model.TestId });
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"An error occurred: {ex.Message}";
                return RedirectToAction("AdminDashboard");
            }
        }





























        [Authorize(Roles = "Admin")]
        public IActionResult CreateQuestionBank()
        {
            ViewBag.Departments = new SelectList(_context.Departments.ToList(), "Id", "DepartmentName");
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateQuestionBank(QuestionBank questionBank)
        {
            if (ModelState.IsValid)
            {
                _context.QuestionBanks.Add(questionBank);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Question bank created successfully!";
                return RedirectToAction("ManageQuestionBanks");
            }

            ViewBag.Departments = new SelectList(_context.Departments.ToList(), "Id", "DepartmentName");
            return View(questionBank);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ManageQuestionBanks()
        {
            var questionBanks = await _context.QuestionBanks.Include(qb => qb.Questions).ToListAsync();
            return View(questionBanks);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> AddQuestionsToBank(int questionBankId)
        {
            var questionBank = await _context.QuestionBanks.FindAsync(questionBankId);
            if (questionBank == null)
            {
                TempData["ErrorMessage"] = "Question bank not found.";
                return RedirectToAction("ManageQuestionBanks");
            }

            ViewBag.QuestionBank = questionBank;
            return View(new AddQuestionViewModel());
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddQuestionsToBank(int questionBankId, AddQuestionViewModel model)
        {
            if (ModelState.IsValid)
            {
                var newQuestion = new Question
                {
                    QuestionBankId = questionBankId,
                    QuestionText = model.QuestionText,
                    OptionA = model.OptionA,
                    OptionB = model.OptionB,
                    OptionC = model.OptionC,
                    OptionD = model.OptionD,
                    CorrectOption = model.CorrectOption
                };

                // Set the question number
                var maxQuestionNumber = await _context.Questions
                    .Where(q => q.QuestionBankId == questionBankId)
                    .MaxAsync(q => (int?)q.QuestionNumber) ?? 0;
                newQuestion.QuestionNumber = maxQuestionNumber + 1;

                _context.Questions.Add(newQuestion);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Question added successfully!";
                return RedirectToAction("ViewQuestionBankQuestions", new { questionBankId });
            }

            ViewBag.QuestionBank = await _context.QuestionBanks.FindAsync(questionBankId);
            return View(model);
        }  




        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ViewQuestionBankQuestions(int questionBankId)
        {
            var questionBank = await _context.QuestionBanks
                .Include(qb => qb.Questions)
                //.Include(qb => qb.Course)
                .FirstOrDefaultAsync(qb => qb.Id == questionBankId);

            if (questionBank == null)
            {
                TempData["ErrorMessage"] = "Question bank not found.";
                return RedirectToAction("ManageQuestionBanks");
            }

            return View(questionBank);
        }






        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteQuestionFromBank(int questionId, int questionBankId)
        {
            var question = await _context.Questions.FindAsync(questionId);
            if (question != null)
            {
                _context.Questions.Remove(question);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Question deleted successfully.";
            }
            return RedirectToAction("ViewQuestionBankQuestions", new { questionBankId = questionBankId });
        }

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteTest(int id)
        {
            var test = await _context.Tests
                .Include(t => t.TestAttempts)
                .FirstOrDefaultAsync(t => t.Id == id);

            if (test == null)
            {
                TempData["ErrorMessage"] = "Test not found.";
                return RedirectToAction("ManageTests");
            }

            if (test.TestAttempts.Any())
            {
                TempData["ErrorMessage"] = "Cannot delete test with existing attempts.";
                return RedirectToAction("ManageTests");
            }

            _context.Tests.Remove(test);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Test deleted successfully.";
            return RedirectToAction("ManageTests");
        }
    }
}
