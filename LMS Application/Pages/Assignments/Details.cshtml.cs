using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.model;

namespace RegisterPage.Pages.Assignments
{
    public class DetailsModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DetailsModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public assignments Assignments { get; set; } = default!;
        public string? Role { get; private set; }
        public string CourseName { get; set; }


        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignments = await _context.assignments.FirstOrDefaultAsync(m => m.ID == id);
            if (assignments == null)
            {
                return NotFound();
            }
            else
            {
                Assignments = assignments;

                // Retrieve the courseName from the Classes table based on classID
                var course = await _context.classes
                    .FirstOrDefaultAsync(c => c.Id == assignments.classID);

                if (course != null)
                {
                    CourseName = course.courseName;
                }

            }

            // Check if the role is already stored in the session
            Role = HttpContext.Session.GetString("UserRole");

            if (string.IsNullOrEmpty(Role))
            {
                // Get the logged-in user's username
                var username = HttpContext.Session.GetString("Username");

                if (username != null)
                {
                    // Retrieve the user's role from the database
                    var user = await _context.register
                        .FirstOrDefaultAsync(u => u.username == username);

                    if (user != null)
                    {
                        Role = user.role;

                        // Store the role in the session
                        HttpContext.Session.SetString("UserRole", Role);
                    }
                }
            }

            return Page();
        }
    }
}
