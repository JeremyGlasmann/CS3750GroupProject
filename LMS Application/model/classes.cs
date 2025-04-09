using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisterPage.model
{

    public class classes
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [ForeignKey("Professor")]  // Specify the foreign key relationship
        public int professorID { get; set; }

        [Required]
        [Display(Name = "Course Name")]
        public string? courseName { get; set; }


        [Required]
        [Display(Name = "Course Type")]
        public string? courseType { get; set; }


        [Required]
        [Display(Name = "Course Number")]
        public string? courseNumber { get; set; }


        [Required]
        [Display(Name = "Credit Hours")]
        public int creditHours { get; set; }

        [Required]
        [Display(Name = "Location")]
        public string? location { get; set; }

        [Display(Name = "Days")]
        public string? days { get; set; }

        [Required]
        [Display(Name = "CRN")]
        [RegularExpression(@"^[0-9]{5}$", ErrorMessage = "CRN must be exactly 5 digits.")]
        public string crn { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:H:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Time")]
        [DataType(DataType.Time)]
        public DateTime startTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yy}", ApplyFormatInEditMode = true)]
        [Display(Name = "Start Date")]
        [DataType(DataType.Date)]
        public DateTime startDate { get; set; }

        [Required]
        [DisplayFormat(DataFormatString = "{0:H:mm}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Time")]
        [DataType(DataType.Time)]
        public DateTime endTime { get; set; }

        [DisplayFormat(DataFormatString = "{0:MM/dd/yy}", ApplyFormatInEditMode = true)]
        [Display(Name = "End Date")]
        [DataType(DataType.Date)]
        public DateTime endDate { get; set; }

        [Required]
        [Display(Name = "Course Size")]
        public int courseSize { get; set; }

        public ICollection<register>? Users { get; set; } = new List<register>();
        public ICollection<assignments>? Assignments { get; set; } = new List<assignments>();
        public ICollection<Notification>? Notifications { get; set; } = new List<Notification>();
    }
}
