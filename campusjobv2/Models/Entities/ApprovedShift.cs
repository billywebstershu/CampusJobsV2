using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class ApprovedShift
    {
        [Key]
        public int Timesheet_ID { get; set; }
        
        [Required]
        public int Offer_ID { get; set; }
        
        [Required]
        [Column(TypeName = "decimal(10, 2)")]
        public decimal Hours_Worked { get; set; }
        
        [Required]
        public bool Status { get; set; } // 0: Pending, 1: Approved
        
        [Required]
        public DateTime Date_Uploaded { get; set; } = DateTime.Now;
        
        public DateTime? Date_Reviewed { get; set; }
        
        [ForeignKey("Offer_ID")]
        public virtual OfferedShift OfferedShift { get; set; } = null!;
            
            
    }
}
