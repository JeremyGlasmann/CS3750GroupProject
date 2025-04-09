using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RegisterPage.model;

namespace RegisterPage.Pages.Registration
{
    public class IndexModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public IndexModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public IList<classes> RegisteredClasses { get; set; } = default!;
        public IList<classes> AvailableClasses { get; set; } = default!;

        public async Task<register> fillClasses()
        {
            // Get the username from the session
            var username = HttpContext.Session.GetString("Username");

            // Fetch the user from the database using the session username
            //Remember to use .Include(u => u.Classes) to retrieve information from the UserClass table.
            var user = await _context.register.Include(u => u.Classes).SingleOrDefaultAsync(u => u.username == username);

            if (user != null)
            {
                //This is how to retrieve from UserClass Table.
                RegisteredClasses = (List<classes>)user.Classes;

                /*
                Fetch classes assigned to the user
                RegisteredClasses = await _context.classes
                    .Where(c => c.professorID == user.Id) // TODO -  Professor ID needs to be changes with the User ID from the UserClasses table
                    .ToListAsync();
                */

                AvailableClasses = await _context.classes
                    .Where(c => !user.Classes.Select(uc => uc.Id).Contains(c.Id))
                    .ToListAsync();
                /*
                // Fetch classes not assigned to the user
                AvailableClasses = await _context.classes
                    .Where(c => c.professorID != user.Id) // TODO - this needs to be filted based on the User ID from UserClasses table
                    .ToListAsync();
                */
            }
            else
            {
                // Handle the case where the user is not found
                RegisteredClasses = new List<classes>();
                AvailableClasses = new List<classes>();
            }
            return user;
        }

        public async Task OnGetAsync()
        {
            await fillClasses();
        }
        public async Task<IActionResult> OnPostDropClassAsync(int id)
        {
            var user = await fillClasses(); // Get the user and their classes
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "User not found.");
                return RedirectToPage(); // Handle user not found
            }

            // Find the class to drop
            var selectedClass = await _context.classes.SingleOrDefaultAsync(c => c.Id == id);
            if (selectedClass == null)
            {
                ModelState.AddModelError(string.Empty, "Class not found.");
                return RedirectToPage(); // Handle class not found
            }

            // Check if the user is enrolled in the class
            if (!user.Classes.Any(c => c.Id == selectedClass.Id))
            {
                ModelState.AddModelError(string.Empty, "You are not enrolled in this class.");
                return RedirectToPage(); // Handle case where the user is not enrolled
            }

            // Remove the class from the user's class list
            user.Classes.Remove(selectedClass); // Use the user's tracked instance
            await _context.SaveChangesAsync(); // Save changes to the database

            // Update session with the new class list
            var classList = await _context.register
                .Include(c => c.Classes)
                .Where(c => c.username == user.username)
                .SelectMany(c => c.Classes)
                .ToListAsync();

            var classJSON = JsonConvert.SerializeObject(classList, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            HttpContext.Session.SetString("Classes", classJSON); // Store updated class list in session

            return RedirectToPage(); // Redirect after successfully dropping the class
        }

        public async Task<IActionResult> OnPostAddClassAsync(int id)
        {
            var user = await fillClasses(); // Get the user and their classes
            if (user == null)
            {
                // Handle case where user is not found
                ModelState.AddModelError(string.Empty, "User not found.");
                return RedirectToPage(); // Or return an error page
            }

            var selectedClass = await _context.classes.SingleOrDefaultAsync(c => c.Id == id);
            if (selectedClass == null)
            {
                ModelState.AddModelError(string.Empty, "Class not found.");
                return RedirectToPage(); // Or handle this case differently
            }

            // Check if the user is already enrolled in the class
            if (user.Classes.Any(c => c.Id == selectedClass.Id))
            {
                ModelState.AddModelError(string.Empty, "You are already enrolled in this class.");
                return RedirectToPage(); // Or handle this case differently
            }

            user.Classes.Add(selectedClass); // Add the class
            await _context.SaveChangesAsync(); // Save changes to the database

            // Update session with the new class list
            var classList = await _context.register
                .Include(c => c.Classes)
                .Where(c => c.username == user.username)
                .SelectMany(c => c.Classes)
                .ToListAsync();

            var classJSON = JsonConvert.SerializeObject(classList, new JsonSerializerSettings
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore,
            });
            HttpContext.Session.SetString("Classes", classJSON); // Store class list in session

            return RedirectToPage(); // Redirect after successfully adding the class
        }

        public async Task<IActionResult> OnPostFilterClassesAsync(string searchQuery, string courseType)
        {
            // poulate the registered classes
            var user = await fillClasses();
            if (user != null)
            {
                //filter available classes
                var filteredClasses = _context.classes.Where(c => !user.Classes.Select(uc => uc.Id).Contains(c.Id));

                //Apply course type filter if provided
                if (!string.IsNullOrEmpty(courseType))
                {
                    filteredClasses = filteredClasses.Where(c => c.courseType == courseType);
                }

                //Apply search query filter if provided
                if (!string.IsNullOrEmpty(searchQuery))
                {
                    //TODO - include professor name in the search query
                    filteredClasses = filteredClasses.Where(c => c.courseName.Contains(searchQuery) || c.courseNumber.Contains(searchQuery));
                }

                // Fetch filtered classes
                AvailableClasses = await filteredClasses.ToListAsync();
            }
            return Page();
        }
    }
}
