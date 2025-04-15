using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class RightToWorkDocument
    {
        [Key]
        public int RTW_ID { get; set; }
        
        [Required]
        public int Student_ID { get; set; }
        
        [Required]
        [StringLength(255)]
        public string Document_URL { get; set; } = string.Empty;
        
        [Required]
        public DateTime Upload_Date { get; set; } = DateTime.Now;
        
        [ForeignKey("Student_ID")]
        public virtual Employee Employee { get; set; } = null!;
    }
}
