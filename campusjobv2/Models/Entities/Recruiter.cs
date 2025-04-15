using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class Recruiter
    {
        [Key]
        public int Recruitment_ID { get; set; }
        
        [Required]
        public int User_ID { get; set; }
        
        [ForeignKey("User_ID")]
        public virtual User User { get; set; } = null!;
        
        // Navigation properties
        public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();
        public virtual ICollection<StudentWorker> StudentWorkers { get; set; } = new List<StudentWorker>();
        public virtual ICollection<OfferedShift> OfferedShifts { get; set; } = new List<OfferedShift>();
    }
}
