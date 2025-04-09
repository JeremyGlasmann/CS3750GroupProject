using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.model;

namespace RegisterPage.Pages.Assignments
{
    public class DeleteModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DeleteModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public assignments Assignments { get; set; } = default!;
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
            return Page();
        }

        public object getRouteVal(int? cid, int? aid)
        {
            var routeVal = new { id = cid, assignid = aid};
            return routeVal;
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var assignments = await _context.assignments.FindAsync(id);
            if (assignments != null)
            {
                var course = await _context.classes
                    .FirstOrDefaultAsync(c => c.Id == assignments.classID);
                Assignments = assignments;
                _context.assignments.Remove(Assignments);
                await _context.SaveChangesAsync();
            }


            return RedirectToPage("/Course", getRouteVal(assignments.classID, id));
        }
    }
}
