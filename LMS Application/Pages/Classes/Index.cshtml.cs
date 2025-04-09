using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Classes
{
    public class IndexModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public IndexModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public IList<classes> classes { get; set; } = default!;

        public async Task OnGetAsync()
        {
            // Get the username from session
            var username = HttpContext.Session.GetString("Username");

            // Fetch the user from the database using the session username
            var user = await _context.register.SingleOrDefaultAsync(u => u.username == username);

            if (user != null)
            {
                // Filter classes assigned to the professor
                classes = await _context.classes
                    .Where(c => c.professorID == user.Id) // Assuming professorID is used to link classes to professors
                    .ToListAsync();
            }
            else
            {
                // Handle the case when the user is not found (optional)
                classes = new List<classes>();
            }
        }
    }
}
