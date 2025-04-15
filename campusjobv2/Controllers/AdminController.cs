using System;
using System.Linq;
using System.Threading.Tasks;
using campusjobv2.Models;
using campusjobv2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;

namespace campusjobv2.Controllers
{
    public class AdminController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ApplicationDbContext context, ILogger<AdminController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet]
        public async Task<IActionResult> Index(string searchTerm = null)
        {
            var adminId = HttpContext.Session.GetInt32("AdminId");
            if (adminId == null) return RedirectToAction("Index", "Login");

            var model = new AdminViewModel
            {
                SearchTerm = searchTerm,
                StudentAccount = new StudentAccountModel
                {
                    AvailableRecruiters = await GetRecruitersList()
                }
            };

            await LoadPendingShifts(model);
            await LoadSearchResults(model, searchTerm);

            return View(model);
        }

[HttpPost]
public async Task<IActionResult> ApproveShift(int shiftId)
{
    _logger.LogInformation($"Attempting to approve shift {shiftId}");
    
    try 
    {
        var offeredShift = await _context.OfferedShifts
            .Include(o => o.Employee)
            .FirstOrDefaultAsync(o => o.Offer_ID == shiftId && o.Status == 1);

        if (offeredShift == null)
        {
            _logger.LogWarning($"Shift {shiftId} not found or not in correct status");
            TempData["Error"] = "Shift not found or not ready for approval";
            return RedirectToAction("Index");
        }

        offeredShift.Status = 2;
        await _context.SaveChangesAsync();
        _logger.LogInformation($"Shift {shiftId} approved successfully");

        var notification = new Notification
        {
            User_ID = offeredShift.Student_ID,
            Message = $"Your shift on {offeredShift.Start_Date.ToString("MM/dd/yyyy")} has been approved",
            Time = DateTime.Now,
            Read = false
        };
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync();

        TempData["Success"] = "Shift approved successfully";
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error approving shift");
        TempData["Error"] = $"Error approving shift: {ex.Message}";
    }

    return RedirectToAction("Index");
}
        [HttpPost]
        public async Task<IActionResult> CreateStudentAccount(StudentAccountModel studentAccount)
        {
            if (!ModelState.IsValid)
            {
                studentAccount.AvailableRecruiters = await GetRecruitersList();
                return View("Index", new AdminViewModel { 
                    StudentAccount = studentAccount,
                    SearchTerm = null
                });
            }

            if (!await _context.Recruiters.AnyAsync(r => r.Recruitment_ID == studentAccount.RecruiterId))
            {
                ModelState.AddModelError("StudentAccount.RecruiterId", "Selected recruiter does not exist");
                studentAccount.AvailableRecruiters = await GetRecruitersList();
                return View("Index", new AdminViewModel { StudentAccount = studentAccount });
            }

            using var transaction = _context.Database.BeginTransaction();
            try
            {
                if (await _context.Users.AnyAsync(u => u.Email == studentAccount.Email))
                {
                    ModelState.AddModelError("StudentAccount.Email", "Email already in use");
                    studentAccount.AvailableRecruiters = await GetRecruitersList();
                    return View("Index", new AdminViewModel { StudentAccount = studentAccount });
                }

                var user = new User
                {
                    First_Name = studentAccount.FirstName,
                    Last_Name = studentAccount.LastName,
                    Email = studentAccount.Email,
                    Password = GenerateTemporaryPassword(),
                    Role = 3,
                    Address = "To be provided"
                };
                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                var employee = new Employee
                {
                    Student_ID = user.User_ID,
                    Recruitment_ID = studentAccount.RecruiterId
                };
                _context.Employees.Add(employee);

                _context.RightToWorkDocuments.Add(new RightToWorkDocument
                {
                    Student_ID = user.User_ID,
                    Document_URL = "pending_upload",
                    Upload_Date = DateTime.Now
                });

                _context.VisaStatuses.Add(new VisaStatus
                {
                    Student_ID = user.User_ID,
                    Status = studentAccount.IsVisaRestricted,
                    ExpiryDate = studentAccount.IsVisaRestricted && studentAccount.VisaExpiryDate.HasValue 
                        ? studentAccount.VisaExpiryDate.Value 
                        : DateTime.Now.AddYears(1)
                });

                await _context.SaveChangesAsync();
                transaction.Commit();

                TempData["Success"] = $"Student account created successfully. Temporary password: {user.Password}";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                _logger.LogError(ex, "Error creating student account");
                TempData["Error"] = $"Error creating account: {ex.Message}";
                return RedirectToAction("Index");
            }
        }

        private async Task LoadPendingShifts(AdminViewModel model)
        {
            model.PendingShifts = await _context.OfferedShifts
                .Include(o => o.Employee)
                .ThenInclude(e => e.Recruiter)
                .ThenInclude(r => r.User)
                .Include(o => o.Recruiter)
                .ThenInclude(r => r.User)
                .Where(o => o.Status == 1)
                .OrderByDescending(o => o.Start_Date)
                .Select(o => new AdminViewModel.PendingShiftRecord
                {
                    ShiftId = o.Offer_ID,
                    StudentId = o.Student_ID,
                    StudentName = $"{o.Employee.User.First_Name} {o.Employee.User.Last_Name}",
                    Recruiter = $"{o.Recruiter.User.First_Name} {o.Recruiter.User.Last_Name}",
                    Date = o.Start_Date,
                    Hours = o.Total_Hours,
                    DateOffered = o.Date_Offered
                })
                .ToListAsync();
        }

        private async Task LoadSearchResults(AdminViewModel model, string searchTerm)
        {
            if (!string.IsNullOrEmpty(searchTerm))
            {
                model.SearchResults = await _context.Users
                    .Include(u => u.Admin)
                    .Include(u => u.Recruiter)
                    .ThenInclude(r => r.Employees)
                    .ThenInclude(e => e.VisaStatuses)
                    .Include(u => u.Recruiter)
                    .ThenInclude(r => r.Employees)
                    .ThenInclude(e => e.RightToWorkDocuments)
                    .Where(u => u.First_Name.Contains(searchTerm) || 
                               u.Last_Name.Contains(searchTerm) || 
                               u.Email.Contains(searchTerm))
                    .Select(u => new AdminViewModel.UserSearchResult
                    {
                        UserId = u.User_ID,
                        FirstName = u.First_Name,
                        LastName = u.Last_Name,
                        Email = u.Email,
                        Role = u.Role == 1 ? "Admin" : u.Role == 2 ? "Recruiter" : "Student",
                        HasDocuments = u.Role == 3 && 
                                     u.Recruiter != null && 
                                     u.Recruiter.Employees.Any(e => e.Student_ID == u.User_ID) && 
                                     u.Recruiter.Employees.First(e => e.Student_ID == u.User_ID)
                                        .RightToWorkDocuments.Any(),
                        VisaStatus = u.Role == 3 && 
                                   u.Recruiter != null && 
                                   u.Recruiter.Employees.Any(e => e.Student_ID == u.User_ID) && 
                                   u.Recruiter.Employees.First(e => e.Student_ID == u.User_ID)
                                      .VisaStatuses.Any(v => v.Status && v.ExpiryDate > DateTime.Now)
                    })
                    .ToListAsync();
            }
        }

        private async Task<List<SelectListItem>> GetRecruitersList()
        {
            return await _context.Recruiters
                .Include(r => r.User)
                .Select(r => new SelectListItem
                {
                    Value = r.Recruitment_ID.ToString(),
                    Text = $"{r.User.First_Name} {r.User.Last_Name}"
                })
                .ToListAsync();
        }

        private string GenerateTemporaryPassword()
        {
            const string validChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890!@#$%^&*";
            var random = new Random();
            var chars = new char[12];
            
            for (int i = 0; i < chars.Length; i++)
            {
                chars[i] = validChars[random.Next(validChars.Length)];
            }
            
            return new string(chars);
        }
    }
}
