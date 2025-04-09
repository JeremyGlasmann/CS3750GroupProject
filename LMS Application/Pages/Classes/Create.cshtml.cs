using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Classes
{
    public class CreateModel : PageModel
    {
        private readonly RegisterPageContext _context;

        public CreateModel(RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public classes classes { get; set; } = default!;

        public IActionResult OnGet()
        {
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

            // Get the username from session
            var username = HttpContext.Session.GetString("Username");

            // Fetch the user from the database using the session username
            var user = await _context.register.SingleOrDefaultAsync(u => u.username == username);

            if (user == null)
            {
                // Handle the case when the user is not found
                Console.WriteLine("User not found in the database.");
                return RedirectToPage("/Account/Login"); // Or any appropriate action
            }

            if (user.Classes == null)
            {
                user.Classes = new List<classes>();
            }

            // Assign the user's ID as the professorID
            classes.professorID = user.Id; // Assuming Id is the field in your register model

            _context.classes.Add(classes);



            user.Classes.Add(classes);
            await _context.SaveChangesAsync();

            var classList = _context.register
                .Include(c => c.Classes)
                .Where(c => c.username == user.username)
                .SelectMany(c => c.Classes)
                .ToList();

            var classJSON = JsonConvert.SerializeObject(classList, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            });
            HttpContext.Session.SetString("Classes", classJSON);

            return RedirectToPage("./Index");
        }
    }
}
