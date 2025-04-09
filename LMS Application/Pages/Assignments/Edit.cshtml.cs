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

namespace RegisterPage.Pages.Assignments
{
    public class EditModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public EditModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        [BindProperty]
        public assignments Assignments { get; set; } = default!;

        [BindProperty]
        public int? selectedClassID { get; set; } = default;

        [BindProperty]
        public string SubmitType { get; set; }

        public async Task<IActionResult> OnGetAsync(int? assignmentid, int classid)
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.register.SingleOrDefault(u => u.username == username);
            selectedClassID = classid;
            if (user == null)
            {
                return RedirectToPage("Index");
            }
            if (classid == null)
            {
                return NotFound();
            }
            
            var assignments =  await _context.assignments.FirstOrDefaultAsync(m => m.ID == assignmentid);
            if (assignments == null)
            {
                return NotFound();
            }
            Assignments = assignments;
           ViewData["classID"] = new SelectList(_context.classes, "Id", "courseName");
            return Page();
        }

        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more information, see https://aka.ms/RazorPagesCRUD.
        public async Task<IActionResult> OnPostAsync()
        {
            var username = HttpContext.Session.GetString("Username");
            var user = _context.register.SingleOrDefault(u => u.username == username);
            if (user == null)
            {
                return RedirectToPage("Index");
            }
            if (selectedClassID != null)
            {
                var selectedClass = _context.classes.SingleOrDefault(c => c.Id == selectedClassID);
                Assignments.courseNum = int.Parse(selectedClass.courseNumber);
                Assignments.classID = selectedClass.Id;
                Assignments.Classes = selectedClass;
                Assignments.submissionType = SubmitType;
            }
            TryValidateModel(Assignments);
            if (!ModelState.IsValid)
            {
                return Page();
            }

            _context.Attach(Assignments).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!assignmentsExists(Assignments.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            var routeVal = new { id = selectedClassID };

            return RedirectToPage("/Course", routeVal);
        }

        private bool assignmentsExists(int id)
        {
            return _context.assignments.Any(e => e.ID == id);
        }
    }
}
