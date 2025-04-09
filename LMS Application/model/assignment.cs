using System.ComponentModel.DataAnnotations;

namespace RegisterPage.model;

public class assignments
{
    [Key]
    public int ID {get; set;}

    [Required]
    [Display(Name = "Title")]
    public string? title { get; set;}

    [Display(Name = "Course Number")]
    public int? courseNum {get; set;}

    [Required]
    [Display(Name = "Description")]
    public string? description { get; set;}

    [Display(Name = "Due Date")]
    public DateTime dueDate {get; set;}

    [Required]
    [Display(Name = "Maximum Grade")]
    public int maxGrade {get; set;}

    [Display(Name = "Submission Type")]
    public string? submissionType {get; set;}

    [Required]
    public int classID { get; set;}

    [Display(Name = "Class")]
    public classes? Classes { get; set; }

    public ICollection<Submission>? Submissions { get; set; } = new List<Submission>();


}
