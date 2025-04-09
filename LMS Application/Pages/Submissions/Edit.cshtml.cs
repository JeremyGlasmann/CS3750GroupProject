using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages.Submissions
{
    public class EditModel : PageModel
    {
        private readonly RegisterPage.Data.RegisterPageContext _context;

        public EditModel(RegisterPage.Data.RegisterPageContext context)
        {
            _context = context;
        }

        public string FileContent { get; set; }

        [BindProperty]
        public Submission Submission { get; set; } = default!;

        public register user { get; set; } = default!;

        public assignments assignment { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var submission =  await _context.Submission.FirstOrDefaultAsync(m => m.ID == id);
            if (submission == null)
            {
                return NotFound();
            }
            Submission = submission;

            // For loading user information
            var User = await _context.register.FirstOrDefaultAsync(x => x.Id == Submission.UserID);
            user = User;

            // For loading assignment information
            var Assignment = await _context.assignments.FirstOrDefaultAsync(x => x.ID == Submission.AssignmentID);
            assignment = Assignment;

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "Submissions", Submission.file);

            if (filePath != null)
            {
                FileContent = await System.IO.File.ReadAllTextAsync(filePath);
            }

            ViewData["AssignmentID"] = new SelectList(_context.assignments, "ID", "description");
            ViewData["UserID"] = new SelectList(_context.register, "Id", "firstname");
            
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

            var Assignment = await _context.assignments.FirstOrDefaultAsync(x => x.ID == Submission.AssignmentID);
            var retrieveClass = await _context.classes.SingleOrDefaultAsync(x => x.Id == Assignment.classID);
            var student = await _context.register.SingleOrDefaultAsync(x => x.Id == Submission.UserID);

            // Create a new notification
            var createNotification = new Notification
            {
                classID = retrieveClass.Id,
                Message = $"{Assignment.title} has been graded.",
                Timestamp = DateTime.Now,
                fromUserID = retrieveClass.professorID, // ID of the professor sending the notification
                toUserID = student.Id // ID of the student receiving the notification
            };

            _context.Notification.Add(createNotification);

            // Check if the student already has a Notifications list and add the notification to it
            if (student.Notifications == null)
            {
                student.Notifications = new List<Notification>();
            }
            student.Notifications.Add(createNotification);

            _context.Attach(Submission).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!SubmissionExists(Submission.ID))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return RedirectToPage("./Index", new { id = Submission.AssignmentID });
        }


        private bool SubmissionExists(int id)
        {
            return _context.Submission.Any(e => e.ID == id);
        }

		public async Task<IActionResult> OnGetDownloadFileAsync(int id)
		{
			var submission = await _context.Submission.FirstOrDefaultAsync(m => m.ID == id);
			if (submission == null)
			{
				return NotFound();
			}

			Submission = submission;

			var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Resources", "Submissions", Submission.file);

			if (!System.IO.File.Exists(filePath))
			{
				return NotFound(); // Return a 404 if the file does not exist
			}

			// Get the file extension to determine the content type (MIME type)
			var fileExtension = Path.GetExtension(filePath).ToLower();
			string contentType = "";

			// Set MIME type based on file extension (you may add more types if needed)
			switch (fileExtension)
			{
				case ".jpg":
				case ".jpeg":
					contentType = "image/jpeg";
					break;
				case ".png":
					contentType = "image/png";
					break;
				case ".pdf":
					contentType = "application/pdf";
					break;
                case ".docx":
                    contentType = "document/docx";
                    break;
			}

			var fileName = Submission.file; // The name that will be given to the file when downloaded

			// Use PhysicalFile to return the file from the server
			return PhysicalFile(filePath, contentType, fileName);
		}
	}
}
