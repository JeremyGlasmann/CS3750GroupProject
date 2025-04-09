using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RegisterPage.Data;
using RegisterPage.Migrations;
using RegisterPage.model;

namespace RegisterPage.Pages.Classes
{
    public class EditModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public EditModel(RegisterPage.Data.RegisterPageContext context)
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

            var classes =  await _context.classes.FirstOrDefaultAsync(m => m.Id == id);
            if (classes == null)
            {
                return NotFound();
            }
            Classes = classes;
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Classes).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!classesExists(Classes.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var username = HttpContext.Session.GetString("Username");
            var user = await _context.register.SingleOrDefaultAsync(u => u.username == username);
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

        private bool classesExists(int id)
        {
            return _context.classes.Any(e => e.Id == id);
        }
    }
}
