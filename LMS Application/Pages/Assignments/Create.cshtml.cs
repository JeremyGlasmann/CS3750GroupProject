using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegisterPage.model;

namespace RegisterPage.Pages.Assignments
{
    public class CreateModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public CreateModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public int selectedClassID { get; set; } = default;
        public List<classes> GrabStudent { get; set; }
        public List<register> MatchStudent { get; set; }

        [BindProperty]
        public string SubmitType { get; set; }

        public IActionResult OnGet(int id)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.register.SingleOrDefault(u => u.username == username);
            selectedClassID = id;
            if (user == null)
            {
                return RedirectToPage("Index");
            }

            // Get the course name for the selected class
            var selectedClass = _context.classes.SingleOrDefault(c => c.Id == selectedClassID);
            if (selectedClass != null)
            {
                // Store the course name in ViewData
                ViewData["CourseName"] = selectedClass.courseName;
            }

            ViewData["classID"] = new SelectList(_context.classes, "Id", "courseName");

            return Page();
        }

        [BindProperty]
        public assignments Assignments { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.register.SingleOrDefault(u => u.username == username);
            if (user == null)
            {
                return RedirectToPage("Index");
            }

            if (selectedClassID != null)
            {
                var selectedClass = await _context.classes.SingleOrDefaultAsync(c => c.Id == selectedClassID);
                if (selectedClass != null)
                {
                    Assignments.courseNum = int.Parse(selectedClass.courseNumber);
                    Assignments.classID = selectedClass.Id;
                    Assignments.Classes = selectedClass;
                    Assignments.submissionType = SubmitType;
                }
            }
            TryValidateModel(Assignments);

            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.assignments.Add(Assignments);
            await _context.SaveChangesAsync();

            MatchStudent = _context.classes
            .Include(c => c.Users)
            .Where(c => c.Id == selectedClassID)
            .SelectMany(c => c.Users)
            .ToList();

            foreach (var student in MatchStudent)
            {
                var createNotification = new Notification
                {
                    classID = selectedClassID,
                    Message = $"{Assignments.title} has been assigned",
                    Timestamp = DateTime.Now,
                    fromUserID = user.Id,
                    toUserID = student.Id
                };
                _context.Notification.Add(createNotification);

                // Check if the student already has a Notifications list and add the notification to it
                if (student.Notifications == null)
                {
                    student.Notifications = new List<Notification>();
                }
                student.Notifications.Add(createNotification);
                await _context.SaveChangesAsync();
            }

            var routeVal = new { id = selectedClassID };

            return RedirectToPage("/Course", routeVal);
        }

    }
}
