using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class Admin
    {
        [Key]
        public int Admin_ID { get; set; }
        
        [Required]
        public int User_ID { get; set; }
        
        [ForeignKey("User_ID")]
        public virtual User User { get; set; } = null!;
    }
}
