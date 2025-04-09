using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http;
using RegisterPage.model;
using RegisterPage.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.CodeAnalysis.Host;
using System.Security.Claims;
using RegisterPage.Migrations;
using Newtonsoft.Json;
using SQLitePCL;

namespace RegisterPage.Pages
{
	public class IndexModel : PageModel
	{
		private readonly IServiceProvider _serviceProvider;

		public IndexModel(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		[BindProperty]
		public string Username { get; set; }

		[BindProperty]
		public string Password { get; set; }

		public string ErrorMessage { get; set; }

		public IActionResult OnPost()
		{
			using (var context = new RegisterPageContext(
				_serviceProvider.GetRequiredService<DbContextOptions<RegisterPageContext>>()))
			{
				// Verify the user credentials
				var user = context.register.SingleOrDefault(u => u.username == Username && u.password == Password);

				if (user != null)
				{

					// This retreives notifications for the user
					// Figure out how to put this in the dropdown
					var notifications = context.register
						.Include(u => u.Notifications)
                        .Where(c => c.username == user.username)
                        .SelectMany(c => c.Notifications)
                        .ToList();

					var classList = context.register
						.Include(c => c.Classes)
                        .Where(c => c.username == user.username)
                        .SelectMany(c => c.Classes)
                        .ToList();

                    var settings = new JsonSerializerSettings
                    {
                        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                    };

                    var classJSON = JsonConvert.SerializeObject(classList, settings);


                    // Store the user ID, Username, and Classes in the session
                    HttpContext.Session.SetInt32("UserId", user.Id);
					HttpContext.Session.SetString("Username", user.username);
                    HttpContext.Session.SetString("Classes", classJSON);

                    //Load Notification Tab and highlight it 

                    // Redirect to the dashboard
                    return RedirectToPage("/Dashboard");
				}
				else
				{
					ErrorMessage = "Invalid username or password.";
					return Page();
				}
			}
		}
	}
}
