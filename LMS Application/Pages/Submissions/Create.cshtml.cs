using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Submissions
{
    public class CreateModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public CreateModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
        ViewData["AssignmentID"] = new SelectList(_context.assignments, "ID", "description");
        ViewData["UserID"] = new SelectList(_context.register, "Id", "firstname");
            return Page();
        }

        [BindProperty]
        public Submission Submission { get; set; } = default!;

        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Submission.Add(Submission);
            await _context.SaveChangesAsync();

            return RedirectToPage("./Index");
        }
    }
}
