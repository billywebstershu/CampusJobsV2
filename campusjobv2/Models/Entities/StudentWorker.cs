using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class StudentWorker
    {
        [Key]
        public int VisaStatus_ID { get; set; }
        
        [Required]
        public int Recruitment_ID { get; set; }
        
        [ForeignKey("Recruitment_ID")]
        public virtual Recruiter Recruiter { get; set; } = null!;
    }
}
