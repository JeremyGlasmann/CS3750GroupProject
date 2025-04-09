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
    public class DetailsModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DetailsModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

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
    }
}
