using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class Employee
    {
        [Key]
        public int Student_ID { get; set; }
        
        [Required]
        public int Recruitment_ID { get; set; }
        
        [ForeignKey("Student_ID")]
        public virtual User User { get; set; }
        
        [ForeignKey("Recruitment_ID")]
        public virtual Recruiter Recruiter { get; set; }
        
        public virtual ICollection<RightToWorkDocument> RightToWorkDocuments { get; set; }
        public virtual ICollection<VisaStatus> VisaStatuses { get; set; }
        public virtual ICollection<OfferedShift> OfferedShifts { get; set; }
    }
}
