using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class VisaStatus
    {
        [Key]
        public int VisaStatusID { get; set; }
        
        [Required]
        public int Student_ID { get; set; }
        
        [Required]
        public bool Status { get; set; } // 0: Invalid, 1: Valid
        
        [Required]
        public DateTime ExpiryDate { get; set; }
        
        [ForeignKey("Student_ID")]
        public virtual Employee Employee { get; set; } = null!;
    }
}
