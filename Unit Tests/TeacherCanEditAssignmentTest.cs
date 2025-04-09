using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using RegisterPage.Data;
using RegisterPage.model;
using RegisterPage.Pages.Assignments;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Unit_Test
{
	[TestClass]
	public class TeacherCanEditAssignmentTest
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

			//add to the database and save changes
			_context.register.Add(teacherUser);
			_context.classes.Add(newCourse);
			_context.assignments.Add(newAssignment);
			_context.SaveChanges();

			//Retrieve the existing submission from the database
			var existingAssignments = _context.assignments.Find(20);

			//Setup the PageModel for editing submission
			_pageModel = new EditModel(_context)
			{
				PageContext = _pageContext,
				Assignments = existingAssignments,
			};

			_pageModel.Assignments.description = "Desc";
			_pageModel.Assignments.maxGrade = 150;
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
			var editedAssignment = _context.assignments.FirstOrDefault(c => c.ID == 20);

			Assert.IsNotNull(editedAssignment, "Submission was not created.");
			Assert.AreEqual(150, editedAssignment.maxGrade, "Grade does not match.");
			Assert.AreEqual("Desc", editedAssignment.description, "Description does not match");
		}
	}
}
