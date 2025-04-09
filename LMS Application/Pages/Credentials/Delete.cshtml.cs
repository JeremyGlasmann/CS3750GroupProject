using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Crendentials
{
    public class DeleteModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DeleteModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public register Register { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var register = await _context.register.FirstOrDefaultAsync(m => m.Id == id);

            if (register == null)
            {
                return NotFound();
            }
            else
            {
                Register = register;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var register = await _context.register.FindAsync(id);
            if (register != null)
            {
                register = register;
                _context.register.Remove(register);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
