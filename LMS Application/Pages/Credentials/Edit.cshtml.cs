using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Credentials
{
    /// <summary>
    /// Handles the logic for editing the profile of a user
    /// </summary>
    public class EditModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        //Constructor for depedency injection of the database context
        public EditModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public register Register { get; set; } = default!;

        //Store teacher or student role
        public string Role { get; set; } 

        /// <summary>
        /// Initial page from the GET request
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //retrieve the user by their Id
            var register = await _context.register.FirstOrDefaultAsync(m => m.Id == id);

            // If the user doesn't exit return Not Found
            if (register == null)
            {
                return NotFound();
            }

            // Assign the retrieved record to the Register property
            Register = register;

            // Store the user's role
            Role = register.role;

            register.confirmpassword = register.password;

            //Return for page rendering
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync(int? id)
        {
            //If the model state is invalid, display the page validation errors
            if (!ModelState.IsValid)
            {
                return Page();
            }

            //Mark the register entity as modified in the context
            _context.Attach(Register).State = EntityState.Modified;

            try
            {
                //Save changes to the database
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                //check if the record still exists
                if (!registerExists(Register.Id))
                {
                    return NotFound();
                }
                else
                {
                    throw; //rethrow for different error
                }
            }

            //after editing redirect to the profile page
            return RedirectToPage("/Profile");
        }

        //Helper method to check if the register entry exists by id
        private bool registerExists(int id)
        {
            return _context.register.Any(e => e.Id == id);
        }
    }
}
