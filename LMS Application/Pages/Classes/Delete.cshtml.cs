using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Classes
{
    public class DeleteModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DeleteModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public classes Classes { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classes = await _context.classes.FirstOrDefaultAsync(m => m.Id == id);

            if (classes == null)
            {
                return NotFound();
            }
            else
            {
                Classes = classes;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var classes = await _context.classes.FindAsync(id);
            var username = HttpContext.Session.GetString("Username");
            var user = await _context.register.Include(u => u.Classes).SingleOrDefaultAsync(u => u.username == username);
            if (classes != null)
            {
                classes = classes;
                _context.classes.Remove(classes);

                if(user != null)
                {
                    user.Classes.Remove(classes);
                }

                await _context.SaveChangesAsync();
            }

            var classList = _context.register
                        .Include(c => c.Classes)
                        .Where(c => c.username == user.username)
                        .SelectMany(c => c.Classes)
                        .ToList();

            var settings = new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            };

            var classJSON = JsonConvert.SerializeObject(classList, settings);
            HttpContext.Session.SetString("Classes", classJSON);

            return RedirectToPage("./Index");
        }
    }
}
