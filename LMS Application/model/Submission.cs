using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisterPage.model
{
    public class Submission
    {

        [Key]
        public int ID { get; set; }

        [ForeignKey("User")]
        [Required]
        public int UserID { get; set; }
        public register? User {  get; set; }

        [ForeignKey("Assignment")]
        [Required]
        public int AssignmentID { get; set; }
        public assignments? Assignment { get; set; }

        [Display(Name = "Grade")]
        public int? grade { get; set; }

        [Display(Name = "Date/Time Submitted")]
        public DateTime submitTime { get; set; }

        [Display(Name = "File Name")]
        public string? file { get; set; }
    }
}
