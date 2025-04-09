using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using RegisterPage.Data;
using RegisterPage.model;

namespace RegisterPage.Pages
{
    public class Profile : PageModel
    {
        private readonly IServiceProvider _serviceProvider;

        public Profile(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }
        public int Id { get; set; }

        [BindProperty]
        public string Username { get; set; }

        [BindProperty]
        public string Password { get; set; }
        public string Role { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public DateTime Age { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? ZipCode { get; set; }
        public string? PhoneNum { get; set; }
        public string ErrorMessage { get; set; }

        //Contains the url to the profile image
        public string ProfileImageURL { get; set; }

        //Used to handle form visibility to update the image
        public bool IsUpdateImageFormVisible { get; set; }

        //Used to handle invalid image names
        public string ImageUploadError { get; set; }

        /// <summary>
        /// Reusable method to populate user data
        /// </summary>
        /// <param name="username"></param>
        private void LoadUserData(string username)
        {
            using (var context = new RegisterPageContext(
            _serviceProvider.GetRequiredService<DbContextOptions<RegisterPageContext>>()))
            {
                var user = context.register.SingleOrDefault(u => u.username == username);
                if (user != null)
                {
                    Id = user.Id;
                    Username = user.username;
                    FirstName = user.firstname;
                    LastName = user.lastname;
                    Role = user.role;
                    Password = user.password;
                    Age = user.dob;
                    Street = user.Street;
                    City = user.City;
                    State = user.State;
                    ZipCode = user.ZipCode;
                    PhoneNum = user.PhoneNum;

                    // Query the profile image from the database
                    var profileImage = context.images.SingleOrDefault(images => images.user_id == Id);
                    ProfileImageURL = profileImage != null ? profileImage.img_path : "PlaceholderImage.jpg";
                }
                else
                {
                    ErrorMessage = "User not found.";
                }
            }
        }

        public IActionResult OnGet()
        {
            //Grab User
            var username = HttpContext.Session.GetString("Username");

            if (string.IsNullOrEmpty(username))
            {
                // If no username is found in session, redirect to login page
                return RedirectToPage("/Index");
            }

            //Call the method to load the user's data
            LoadUserData(username);


            //Initialize image form Visibility as false 
            IsUpdateImageFormVisible = false;

            return Page();
        }

        /*Method to toggle the visibility of the form*/
        public IActionResult OnPostToggleForm()
        {
            //Check if the form is currently visible
            IsUpdateImageFormVisible = !IsUpdateImageFormVisible;

            //Grab User
            var username = HttpContext.Session.GetString("Username");

            if (!string.IsNullOrEmpty(username))
            {
                LoadUserData(username);
            }
            return Page();

        }

        /*Method to handle file upload*/
        public IActionResult OnPostUploadImage(IFormFile ProfileImageFile)
        {
            //Set the value equal to the user Id from the session variable
            var idFromSession = HttpContext.Session.GetInt32("UserId");

            //hold the user's username from http
            var username = HttpContext.Session.GetString("Username");


            //handle errors if there is no session Id
            if (idFromSession == null)
            {
                ImageUploadError = "Session expired or user not logged in.";
                return RedirectToPage("/Index");
            }

            //set the user Id from the sessionId
            Id = idFromSession.Value;

            // Handle if there was no submisison
            if (ProfileImageFile == null)
            {
                //Display Error Message if no file was uploaded
                ImageUploadError = "Please Select a file to upload";

                //Replace the profile picture with the placeholder image
                ProfileImageURL = "PlaceholderImage.jpg";
                IsUpdateImageFormVisible = true;

                //Load the user's information to keep it present on the page
                if (!string.IsNullOrEmpty(username))
                {
                    LoadUserData(username);
                }

                //return the page with the error
                return Page();
            }


            //Build the location where the upload image will be saved
            var filePath = Path.Combine("wwwroot/Resources/ProfileImages", ProfileImageFile.FileName);

            //Update the ImageURL to the file
            ProfileImageURL = ProfileImageFile.FileName;

            //If the file already exists in the folder display not available
            if (System.IO.File.Exists(filePath))
            {
                //Display Error Message
                ImageUploadError = "Image name unavailable, please rename your image and try again.";

                //Replace the profile picture with the placeholder image
                ProfileImageURL = "PlaceholderImage.jpg";
                IsUpdateImageFormVisible = true; // Keep the form visible

                //Load the user's information to keep it present on the page
                if (!string.IsNullOrEmpty(username))
                {
                    LoadUserData(username);
                }

                //return the page with the error
                return Page();
            }


            //Save the uploaded file if it doesn't exist in the folder
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                ProfileImageFile.CopyTo(stream);
            }

            //Connect to the database for iamges
            using (var context = new RegisterPageContext(_serviceProvider.GetRequiredService<DbContextOptions<RegisterPageContext>>()))
            {
                var profileImage = context.images.SingleOrDefault(images => images.user_id == Id);

                if (profileImage != null)
                {
                    //Update the existing record with the new image path
                    profileImage.img_path = ProfileImageFile.FileName;
                    context.images.Update(profileImage);
                }
                else
                {
                    var newProfileImage = new ProfileImage
                    {
                        img_path = ProfileImageFile.FileName,
                        user_id = Id
                    };
                    context.images.Add(newProfileImage);
                }
                //Save changes to the database
                try
                {
                    context.SaveChanges();
                }
                catch (DbUpdateException ex)
                {
                    //Handle upload errors
                    ImageUploadError = $"An error occured while updating the image. Error Message: {ex.Message}";
                    IsUpdateImageFormVisible = true;
                    return Page();
                }
            }



            //Reset form visibility after upload
            IsUpdateImageFormVisible = false;

            //Update the image upload name
            ImageUploadError = "Image updated successfully";

            //Load the user's information to keep it present on the page
            if (!string.IsNullOrEmpty(username))
            {
                LoadUserData(username);
            }


            //return the page
            return Page();
        }
    }
}
