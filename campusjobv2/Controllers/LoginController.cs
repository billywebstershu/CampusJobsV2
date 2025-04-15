using campusjobv2.Models;
using campusjobv2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace campusjobv2.Controllers
{
    public class LoginController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<LoginController> _logger;

        public LoginController(ApplicationDbContext context, ILogger<LoginController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public IActionResult Index()
        {
            return View(new LoginViewModel());
        }

        [HttpPost]
        public async Task<IActionResult> Index(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == model.Email && u.Role == model.Role);

            if (user == null || user.Password != model.Password) 
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }


            HttpContext.Session.Clear();


            HttpContext.Session.SetInt32("UserId", user.User_ID);
            HttpContext.Session.SetInt32("UserRole", user.Role);

           
            switch (user.Role)
            {
                case 1: // Admin
                    var admin = await _context.Admins.FirstOrDefaultAsync(a => a.User_ID == user.User_ID);
                    if (admin != null) HttpContext.Session.SetInt32("AdminId", admin.Admin_ID);
                    return RedirectToAction("Index", "Admin");
                    
                case 2: // Recruiter
                    var recruiter = await _context.Recruiters.FirstOrDefaultAsync(r => r.User_ID == user.User_ID);
                    if (recruiter != null) HttpContext.Session.SetInt32("RecruiterId", recruiter.Recruitment_ID);
                            Console.WriteLine($"Set RecruiterId: {recruiter.Recruitment_ID}");
                    return RedirectToAction("Index", "Recruiter");
                    
                case 3: // Student
                    var studentId = (await _context.Employees
                        .FirstOrDefaultAsync(e => e.Student_ID == user.User_ID))?.Student_ID ?? user.User_ID;
                    HttpContext.Session.SetInt32("StudentId", studentId);
                    return RedirectToAction("Index", "TimeSheets");
                    
                default:
                    return RedirectToAction("Index");
            }
        }

        [HttpGet]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Index");
        }
    }
}
