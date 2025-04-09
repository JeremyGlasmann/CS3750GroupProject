using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.model;

namespace RegisterPage.Pages.Submissions
{
    public class IndexModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public Submission submission { get; set; } = default!;

        public assignments assignment { get; set; } = default!;

        public string courseNum { get; set; } = string.Empty;



        public IndexModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public IList<Submission> Submission { get; set; } = default!;

        public async Task OnGetAsync(int id)
        {

            var submissions = await _context.Submission.FirstOrDefaultAsync(m => m.ID == id);
            submission = submissions;

            var Assignment = await _context.assignments.FirstOrDefaultAsync(m => m.ID == id);
            assignment = Assignment;

            var assignments = await _context.assignments
                .Include(a => a.Classes)
                .Include(a => a.Submissions)
                .ThenInclude(s => s.User)
                .SingleOrDefaultAsync(s => s.ID == id);

            assignment = assignments;

            // Filter submissions for the assignment
            if (assignments != null)
            {
                //Assign Course number
                courseNum = assignment.Classes.courseNumber;

                Submission = assignments.Submissions
                .OrderBy(s => s.User.firstname)
                .ThenBy(s => s.User.lastname)
                .ToList();
            }

        }
    }
}
