using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Credentials
{
    public class CreateModel : PageModel
    {
        private readonly RegisterPageContext _context;

        public CreateModel(RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public register register { get; set; } = new register();

        [BindProperty]
        public string SelectedRole { get; set; } = "Student";

        public IList<classes> Classes { get; set; } = new List<classes>();

        public async Task<IActionResult> OnGetAsync()
        {
            // No need to fetch classes for registration
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                // Log or inspect errors if validation fails
                foreach (var error in ModelState)
                {
                    foreach (var subError in error.Value.Errors)
                    {
                        Console.WriteLine($"Error in {error.Key}: {subError.ErrorMessage}");
                    }
                }

                return Page();
            }

            // Set role and save register entry
            register.role = SelectedRole;
            _context.register.Add(register);
            await _context.SaveChangesAsync();

            return RedirectToPage("/Index");
        }
    }
}
