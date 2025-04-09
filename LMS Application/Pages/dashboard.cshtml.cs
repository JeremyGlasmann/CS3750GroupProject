using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using RegisterPage.Data;
using RegisterPage.model;
using System.IO;

namespace RegisterPage.Pages
{
    public class Dashboard : PageModel
    {
        private readonly IServiceProvider _serviceProvider;

        public Dashboard(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Role { get; set; }
        public List<model.classes> ClassList { get; set; }
        public List<ToDoItem> ToDoList { get; set; }
        public string ErrorMessage { get; set; }

        public class ToDoItem
        {
            public int Id { get; set; } 

            public string Name { get; set; }
            public DateTime DueDate { get; set; }
            public string CourseID { get; set; }
            public string ClassCode { get; set; }
        }

        public IActionResult OnGet()
        {
            var username = HttpContext.Session.GetString("Username");
            var classJSON = HttpContext.Session.GetString("Classes");

            if (string.IsNullOrEmpty(username))
            {
                return RedirectToPage("/Index");
            }
            using (var context = new RegisterPageContext(
                _serviceProvider.GetRequiredService<DbContextOptions<RegisterPageContext>>()))
            {
                var user = context.register.SingleOrDefault(u => u.username == username);
                if (user != null)
                {
                    FirstName = user.firstname;
                    LastName = user.lastname;
                    Role = user.role;

                    if (!string.IsNullOrEmpty(classJSON))
                    {
                        ClassList = JsonConvert.DeserializeObject<List<classes>>(classJSON);
                    }

                    if (Role == "Student")
                    {
                        DateTime currentDate = DateTime.Now;

                        ToDoList = context.assignments
                            .Include(c => c.Classes)
                            .Where(a => ClassList.Select(c => c.Id).Contains(a.classID)
                                    && a.dueDate >= currentDate) // Ensure assignment is not late
                            .OrderBy(a => a.dueDate)
                            .Take(5)
                            .Select(a => new ToDoItem
                            {
                                Id = a.ID, // Include the assignment ID
                                Name = a.title,
                                DueDate = a.dueDate,
                                CourseID = a.classID.ToString(), // Assuming you want to display the class ID
                                ClassCode = a.Classes.courseType + " " + a.Classes.courseNumber,
                            })
                            .ToList();
                    }
                }
                else
                {
                    ErrorMessage = "User not found.";
                }
            }

            return Page();
        }
    }
}
