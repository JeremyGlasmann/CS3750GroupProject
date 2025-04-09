using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Credentials
{
    public class IndexModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public IndexModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public IList<register> register { get;set; } = default!;

        public async Task OnGetAsync()
        {
            register = await _context.register.ToListAsync();
        }
    }
}
