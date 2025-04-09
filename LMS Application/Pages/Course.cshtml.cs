using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.model;
using RegisterPage.Services;

namespace RegisterPage.Pages
{
    public class CourseModel : PageModel
    {
        /// <summary>
        /// Database context to interact with the database
        /// </summary>
        private readonly RegisterPage.Data.RegisterPageContext _context;

        /// <summary>
        /// Constructor to initialize the context
        /// </summary>
        /// <param name="context"></param>
        public CourseModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public int Id { get; set; }
        public IList<register> Students { get; set; } = new List<register>();
        public int pointsPossible { get; set; }
        public int studentScore { get; set; }

        public IList<assignments> assignments { get; set; } = default!;
        public assignments currentAssignment { get; set; } = default!;
        public classes currentClass { get; set; } = default!;
        public string Role { get; set; } = default;
        public string fileType { get; set; }
        public IPathResolver pathResolver { get; set; }

        [BindProperty]
        public int classID { get; set; }

        [BindProperty]
        public int assignID { get; set; }

        // Return correct values in url to course
        public object getRouteVal()
        {
            var Class = _context.classes.Include(x => x.Assignments).SingleOrDefault(c => c.Id == classID);
            var Assignment = Class.Assignments.SingleOrDefault(a => a.ID == assignID);
            var routeVal = new { id = classID, assignId = Assignment.ID };
            return routeVal;
        }

        public async Task createSubmission(IFormFile file)
        {
            string[] validFormats = { ".txt", ".pdf", ".docx", ".png", "jpeg", "jpg" };
            string fileExtension = System.IO.Path.GetExtension(file.FileName);

            if (file == null || !validFormats.Any(words => fileExtension.Contains(words)))
            {
                return;
            }
            var userID = HttpContext.Session.GetInt32("UserId") ?? default(int);

            var Class = await _context.classes.Include(x => x.Assignments).SingleOrDefaultAsync(c => c.Id == classID);
            var a = Class.Assignments.SingleOrDefault(a => a.ID == assignID);
            var routeVal = new { id = classID, assignId = a.ID };

            var filePath = "wwwroot/Resources/Submissions";
            var uniqueFileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var fullPath = Path.Combine(filePath, uniqueFileName);

            var submission = await _context.Submission
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .SingleOrDefaultAsync(s => s.UserID == userID && s.AssignmentID == a.ID);

            if (submission == null)
            {
                submission = new Submission();
                submission.AssignmentID = a.ID;
                submission.Assignment = a;
                submission.file = uniqueFileName;
                submission.submitTime = DateTime.Now;
                submission.UserID = userID;
                submission.User = _context.register.SingleOrDefault(u => u.Id == submission.UserID);
                _context.Submission.Add(submission);
            }
            else
            {
                if (submission.file != null)
                {
                    System.IO.File.Delete(Path.Combine(filePath, submission.file));
                }
                submission.file = uniqueFileName;
                submission.submitTime = DateTime.Now;

            }

            using (var stream = new FileStream(fullPath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }
            await _context.SaveChangesAsync();
            if (TempData != null)
            {
                TempData["ConfirmationMessage"] = "File Submitted!";
            }

        }

        public async Task createSubmission(string text)
        {
            if (text == null)
            {
                return;
            }
            var userID = HttpContext.Session.GetInt32("UserId") ?? default(int);
            var filePath = "wwwroot/Resources/Submissions";
            var uniqueFileName = $"{Guid.NewGuid()}.txt";
            if (pathResolver == null)
            {
                pathResolver = new PathResolverService();
            }
            var fullPath = pathResolver.ResolvePath(uniqueFileName);

            var Class = await _context.classes.Include(x => x.Assignments).SingleOrDefaultAsync(c => c.Id == classID);
            var a = Class.Assignments.SingleOrDefault(a => a.ID == assignID);
            var routeVal = new { id = classID, assignId = a.ID };

            var submission = await _context.Submission
                .Include(x => x.Assignment)
                .Include(x => x.User)
                .SingleOrDefaultAsync(s => s.UserID == userID && s.AssignmentID == a.ID);

            if (submission == null)
            {
                submission = new Submission();
                submission.AssignmentID = a.ID;
                submission.Assignment = a;
                submission.file = uniqueFileName;
                submission.submitTime = DateTime.Now;
                submission.UserID = userID;
                submission.User = _context.register.SingleOrDefault(u => u.Id == submission.UserID);
                _context.Submission.Add(submission);
            }
            else
            {
                submission.file = uniqueFileName;
                submission.submitTime = DateTime.Now;
            }


            System.IO.File.WriteAllText(fullPath, text);
            await _context.SaveChangesAsync();
            if (TempData != null)
            {
                TempData["ConfirmationMessage"] = "File Submitted!";
            }
        }

        // Method to calculate the total points for a student
        public async Task<int> CalculateStudentScore(int userID)
        {
            // Get all assignment IDs for the given class
            var assignmentIds = await _context.assignments
                .Where(a => a.classID == classID)
                .Select(a => a.ID)
                .ToListAsync();

            // Sum up the grades for the given user ID and filtered assignment IDs
            var totalPoints = await _context.Submission
                .Where(s => assignmentIds.Contains(s.AssignmentID) && s.UserID == userID)
                .SumAsync(s => s.grade); // Assuming 'Grade' is the property in Submission

            return (int)totalPoints;
        }

        public async Task<int?> GetAssignmentGrade(int userId, int assignmentId)
        {
            // Check if a submission with a grade exists for this user and assignment
            var submission = await _context.Submission
                .Where(s => s.UserID == userId && s.AssignmentID == assignmentId)
                .Select(s => s.grade)
                .FirstOrDefaultAsync();

            return submission; // Will be null if no submission or grade found
        }


        public async Task OnGetAsync(int? id, int? assignid)
        {
            if (id == null)
            {
                return;
            }
            classID = id.Value;
            var username = HttpContext.Session.GetString("Username");
            var user = _context.register.SingleOrDefault(u => u.username == username);
            if (user != null)
            {
                Role = user.role;
            }

            // Ensure assignments are eagerly loaded with the class
            var Class = await _context.classes
                .Include(c => c.Assignments) // Include the assignments
                .SingleOrDefaultAsync(c => c.Id == id);
            currentClass = Class;

            if (Class != null)
            {
                assignments = Class.Assignments.ToList(); // Use the loaded assignments
            }

            if (assignid != null)
            {
                assignID = assignid.Value;
                // Find the current assignment using the loaded list
                var a = Class.Assignments.SingleOrDefault(a => a.ID == assignID);
                currentAssignment = a;
                if (a != null)
                {
                    fileType = a.submissionType;
                }
            }

            //List of students in the class
            Students = await _context.register.Where(r => r.Classes.Any(c => c.Id == classID) && r.role != "Teacher").ToListAsync();

            // Calculate total possible points for the course
            pointsPossible = assignments.Sum(a => a.maxGrade);

            // Calculate the total points from each student
            foreach (var student in Students)
            {
                studentScore = await CalculateStudentScore(student.Id);
            }

        }


        public async Task<IActionResult> OnPostAsync()
        {
            return RedirectToPage("/Course", getRouteVal());
        }

        public async Task<IActionResult> OnPostUploadFileAsync(IFormFile file)
        {
            await createSubmission(file);

            return RedirectToPage("/Course", getRouteVal());
        }

        public async Task<IActionResult> OnPostUploadTextAsync(string text)
        {
            await createSubmission(text);

            return RedirectToPage("/Course", getRouteVal());
        }
    }
}
