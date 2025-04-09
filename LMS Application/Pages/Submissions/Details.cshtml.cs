using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Submissions
{
    public class DetailsModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DetailsModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public Submission Submission { get; set; } = default!;

        public register user { get; set; } = default!;

        public assignments assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submission.FirstOrDefaultAsync(m => m.ID == id);
            if (submission == null)
            {
                return NotFound();
            }
            else
            {
                Submission = submission;

                var User = await _context.register.FirstOrDefaultAsync(x => x.Id == Submission.UserID);
                user = User;

                var Assignment = await _context.assignments.FirstOrDefaultAsync(x => x.ID == Submission.AssignmentID);
                assignment = Assignment;
            }
            return Page();
        }
    }
}
