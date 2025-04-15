using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class User
    {
        [Key]
        public int User_ID { get; set; }
        
        [Required]
        [StringLength(255)]
        public string First_Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Last_Name { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;
        
        [Required]
        [StringLength(255)]
        public string Password { get; set; } = string.Empty;
        
        [Required]
        public int Role { get; set; } // 1: Admin, 2: Recruiter, 3: Student
        
        [StringLength(255)]
        public string? Address { get; set; }
        
        // Navigation properties
        public virtual Admin? Admin { get; set; }
        public virtual Recruiter? Recruiter { get; set; }
        public virtual ICollection<Notification> Notifications { get; set; } = new List<Notification>();
    }
}
