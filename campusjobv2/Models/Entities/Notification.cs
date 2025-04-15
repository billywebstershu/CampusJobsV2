using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace campusjobv2.Models.Entities
{
    public class Notification
    {
        [Key]
        public int Notification_ID { get; set; }
        
        [Required]
        public int User_ID { get; set; }
        
        [Required]
        public string Message { get; set; } = string.Empty;
        
        [Required]
        public DateTime Time { get; set; } = DateTime.Now;
        
        [Required]
        public bool Read { get; set; } = false;
        
        [ForeignKey("User_ID")]
        public virtual User User { get; set; } = null!;
    }
}
