using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisterPage.model;

public class Notification
{
    [Key]
    public int ID {get; set;}

    [Required]
    [ForeignKey("Classes")]
    public int classID { get; set; }
    [Display(Name = "Class")]
    public classes? Classes { get; set; }

    [Required]
    [ForeignKey("fromUserID")]
    public int fromUserID { get; set; }


    [Required]
    [ForeignKey("toUserID")]
    public int toUserID { get; set; }

    [Required]  //Add a Validation check?
    public string Message {get; set;}
    public DateTime Timestamp {get; set;}

    public ICollection<register>? Users { get; set; } = new List<register>();
}