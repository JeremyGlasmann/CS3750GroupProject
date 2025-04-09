using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages_Receipt
{
    public class DeleteModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public DeleteModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public Receipts Receipts { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipts = await _context.Receipts.FirstOrDefaultAsync(m => m.Id == id);

            if (receipts == null)
            {
                return NotFound();
            }
            else
            {
                Receipts = receipts;
            }
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var receipts = await _context.Receipts.FindAsync(id);
            if (receipts != null)
            {
                Receipts = receipts;
                _context.Receipts.Remove(Receipts);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage("./Index");
        }
    }
}
