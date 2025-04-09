using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using static RegisterPage.model.FutureDateAttribute;

namespace RegisterPage.model
{
    public class register
    {
        [Key]
        public int Id { get; set; }

        [Required(ErrorMessage = "Email name is requried")]
        [DisplayName("Email")]
        [EmailValidation]
        public string? username { get; set; }

        [Required(ErrorMessage = "Password is requried")]
        [DisplayName("Password")]
        public string? password { get; set; }

        [Required(ErrorMessage = "Confirm Password is requried")]
        [Compare(nameof(password), ErrorMessage = "Passwords don't match.")]
        public string confirmpassword { get; set; }

        [Required(ErrorMessage = "First name is requried")]
        public string? firstname { get; set; }

        [Required(ErrorMessage = "Last name is requried")]
        public string? lastname { get; set; }

        [Required(ErrorMessage = "Date of birth is requried")]
        [DataType(DataType.Date)]
        [MinimumAge(16)]
        public DateTime dob { get; set; }

        [Required]
        public string role { get; set; }

        public ICollection<classes>? Classes { get; set; } = new List<classes>();
        public ICollection<Submission>? Submissions { get; set; } = new List<Submission>();
        public ICollection<Receipts>? Receipts { get; set; } = new List<Receipts>();
        public ICollection<Notification>? Notifications { get; set; } = new List<Notification>();

        /*THIS IS THE INFORMATION FOR PROFILE PAGE*/
        /*Address*/
        public string? Street { get; set; } = string.Empty;
        public string? City { get; set; } = string.Empty;
        public string? State { get; set; } = string.Empty;
        [StringLength(5)]

        //Regex to validate zip code 5 numbers long
        [RegularExpression(@"^\d{5}(?:[-\s]\d{4})?$", ErrorMessage = "Invalid zip code format.")]
        public string? ZipCode { get; set; } = string.Empty;

        [Phone(ErrorMessage = "Invalid phone number format.")]
        [RegularExpression("^([0-9]{10})$", ErrorMessage = "Invalid phone number format.")]
        public string? PhoneNum { get; set; } = null;
        //Need to add validation for phone and maybe zip? 


    }

    public class EmailValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var email = value as string;

            if (string.IsNullOrEmpty(email))
            {
                return new ValidationResult("Email is required.");
            }

            // Example custom validation logic
            if (!email.Contains("@") || !email.Contains("."))
            {
                return new ValidationResult("Invalid email address format.");
            }

            return ValidationResult.Success;
        }
    }

    public class FutureDateAttribute : ValidationAttribute
    {
        public class MinimumAgeAttribute : ValidationAttribute
        {
            private readonly int _minimumAge;

            public MinimumAgeAttribute(int minimumAge)
            {
                _minimumAge = minimumAge;
            }

            protected override ValidationResult IsValid(object value, ValidationContext validationContext)
            {
                if (value is DateTime dob)
                {
                    // Calculate age
                    var today = DateTime.Today;
                    var age = today.Year - dob.Year;

                    if (dob.Date > today.AddYears(-age)) age--;

                    if (age < _minimumAge)
                    {
                        return new ValidationResult($"You must be at least {_minimumAge} years old.");
                    }

                    return ValidationResult.Success;
                }

                return new ValidationResult("Invalid date format.");
            }
        }
    }
}
