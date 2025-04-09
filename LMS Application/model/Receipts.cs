using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace RegisterPage.model
{
    public class Receipts
    {
        [Key]
        public int Id { get; set; }
        [ForeignKey("User")]
        public int UserId {get; set;}
        public string? Username { get; set; } = string.Empty;
        public int TotalDue { get; set; }
        public int TotalPaid {get; set;} = 0;
        [DataType(DataType.Date)]
        public DateTime PaidDate {get; set;}
    }
}