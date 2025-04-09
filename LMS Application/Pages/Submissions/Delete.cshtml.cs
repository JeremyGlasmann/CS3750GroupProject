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
    public class DeleteModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DeleteModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Submission Submission { get; set; } = default!;

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
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission = await _context.Submission.FindAsync(id);
            if (submission != null)
            {
                Submission = submission;
                _context.Submission.Remove(Submission);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
