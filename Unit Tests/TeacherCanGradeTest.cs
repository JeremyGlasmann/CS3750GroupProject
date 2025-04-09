
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegisterPage.Data;
using RegisterPage.model;
using RegisterPage.Pages.Submissions;


namespace Unit_Test
{
	[TestClass]
	public class TeacherCanGradeTest
	{
		private DefaultHttpContext _httpContext;
		private PageContext _pageContext;
		private RegisterPageContext _context;
		private EditModel _pageModel;

		// Configure in-memory database for testing
		public static DbContextOptions<RegisterPageContext> TestDbContextOptions()
		{
			// Create a new service provider to create a new in-memory database.
			var serviceProvider = new ServiceCollection()
				.AddEntityFrameworkInMemoryDatabase()
				.BuildServiceProvider();

			// Build the Db context options with in-memory database
			var builder = new DbContextOptionsBuilder<RegisterPageContext>()
				.UseInMemoryDatabase("InMemoryDb")
				.UseInternalServiceProvider(serviceProvider);

			return builder.Options;
		}

		// Initialize method to set up test context before each test
		[TestInitialize]
		public void Init()
		{
			//create new HTTP instance of in-memory database
			_httpContext = new DefaultHttpContext();
			_context = new RegisterPageContext(TestDbContextOptions());
			_pageContext = new PageContext
			{
				HttpContext = _httpContext
			};

			// create a sample teacher user profile
			var teacherUser = new register
			{
				Id = 20,
				firstname = "Teacher10",
				lastname = "Ten",
				username = "teacher10@weber.edu",
				password = "pass",
				confirmpassword = "pass",
				role = "Teacher"
			};

			// create a sample student user profile
			// create a sample student user profile
			var studentUser = new register
			{
				Id = 21,
				firstname = "Student10",
				lastname = "Ten",
				username = "student@10.mail.weber.edu",
				password = "pass",
				confirmpassword = "pass",
				role = "Student"
			};

			// create a new course 
			var newCourse = new classes
			{
				Id = 20,
				professorID = 20,
				courseName = "TestCourse",
				courseType = "Test",
				courseNumber = "9999",
				creditHours = 4,
				location = "Noorda Engineering",
				days = "MWF",
				crn = "99999",
				startTime = new DateTime(2024, 8, 26, 8, 30, 0), // August 26, 2024, 8:30 AM
				endTime = new DateTime(2024, 8, 26, 9, 30, 0),   // August 26, 2024, 9:30 AM
				startDate = new DateTime(2024, 8, 26),            // August 26, 2024
				endDate = new DateTime(2024, 12, 6),              // December 6, 2024
				courseSize = 30
			};

			// create a new assignment for the course
			var newAssignment = new assignments
			{
				ID = 20,
				title = "TestAssignment",
				courseNum = 9999,
				description = "TEST",
				dueDate = new DateTime(2024, 11, 26, 11, 59, 0), // November 26, 2024, 11:59 AM
				maxGrade = 100,
				submissionType = "Text",
				classID = 20,
			};

			// create a new student submission 
			var newSubmission = new Submission
			{
				ID = 20,
				UserID = 21,
				AssignmentID = 20,
				submitTime = new DateTime(2024, 11, 26, 11, 58, 0), // November 26, 2024, 11:58 AM
				grade = 50
			};

			//add to the database and save changes
			_context.register.Add(teacherUser);
			_context.register.Add(studentUser);
			_context.classes.Add(newCourse);
			_context.assignments.Add(newAssignment);
			_context.Submission.Add(newSubmission);
			_context.SaveChanges();

			//Retrieve the existing submission from the database
			var existingSubmission = _context.Submission.Find(20);

			//Setup the PageModel for editing submission
			_pageModel = new EditModel(_context)
			{
				PageContext = _pageContext,
				Submission = existingSubmission,
			};

			_pageModel.Submission.grade = 97;
		}


		//Cleanup method to clear the in-memory database after each test
		[TestCleanup]
		public void Dispose()
		{
			_context.Database.EnsureDeleted();
			_context.Dispose();
		}

		[TestMethod]
		public async Task TeacherCanGrade()
		{

			//call to edit submission grade
			var result = _pageModel.OnPostAsync();

			// Check if the course was added with the correct teacher id
			var gradedSubmission = _context.Submission.FirstOrDefault(c => c.AssignmentID == 20);

			Assert.IsNotNull(gradedSubmission, "Submission was not created.");
			Assert.AreEqual(97, gradedSubmission.grade, "Grade does not match.");
		}
	}
}
