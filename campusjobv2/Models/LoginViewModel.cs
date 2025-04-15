using System.ComponentModel.DataAnnotations;

namespace campusjobv2.Models
{
    public class LoginViewModel
    {
        [Required]
        [EmailAddress]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int Role { get; set; } // 1: Admin, 2: Recruiter, 3: Student
    }
}
