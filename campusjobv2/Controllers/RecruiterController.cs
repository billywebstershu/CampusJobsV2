using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using campusjobv2.Models;
using campusjobv2.Models.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace campusjobv2.Controllers
{
    public class RecruiterController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RecruiterController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var recruiterId = HttpContext.Session.GetInt32("RecruiterId");
            if (recruiterId == null) return RedirectToAction("Index", "Login");

            var model = new RecruiterViewModel
            {
                AvailableStudents = await GetStudents(recruiterId.Value),
                PendingStudentApprovalShifts = await GetPendingStudentShifts(recruiterId.Value),
                PendingAdminApprovalShifts = await GetPendingAdminShifts(recruiterId.Value),
                ApprovedShifts = await GetApprovedShifts(recruiterId.Value)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateShift(DateTime shiftDate, DateTime startTime, DateTime endTime, decimal duration, int studentId)
        {
            var recruiterId = HttpContext.Session.GetInt32("RecruiterId");
            if (recruiterId == null) return RedirectToAction("Index", "Login");

            if (!await ValidateStudent(studentId, recruiterId.Value))
                return RedirectWithError("Student not found or not assigned to you");

            var hasVisaRestriction = await _context.VisaStatuses
                .AnyAsync(v => v.Student_ID == studentId && v.Status && v.ExpiryDate > DateTime.Now);

            if (hasVisaRestriction && duration > 15)
            {
                return RedirectWithError("Cannot create shift that exceeds 15 hours for visa-restricted student");
            }

            try 
            {
                var shift = new OfferedShift
                {
                    Student_ID = studentId,
                    Recruitment_ID = recruiterId.Value,
                    Date_Offered = DateTime.Now,
                    Status = 0,
                    Start_Date = shiftDate.Date.Add(startTime.TimeOfDay),
                    End_Date = shiftDate.Date.Add(endTime.TimeOfDay),
                    Total_Hours = duration
                };

                _context.OfferedShifts.Add(shift);
                await _context.SaveChangesAsync();

                var notification = new Notification
                {
                    User_ID = studentId,
                    Message = $"New shift offered for {shiftDate.ToString("dd/MM/yyyy")} from {startTime.ToString("hh:mm tt")} to {endTime.ToString("hh:mm tt")}",
                    Time = DateTime.Now,
                    Read = false
                };
                _context.Notifications.Add(notification);
                await _context.SaveChangesAsync();

                return RedirectWithSuccess($"Shift created for student ID: {studentId}");
            }
            catch (Exception ex)
            {
                return RedirectWithError($"Error creating shift: {ex.Message}");
            }
        }

        [HttpPost]
        public async Task<IActionResult> CancelShift(int id)
        {
            var recruiterId = HttpContext.Session.GetInt32("RecruiterId");
            if (recruiterId == null) return RedirectToAction("Index", "Login");

            var shift = await _context.OfferedShifts
                .Include(o => o.Employee.User)
                .FirstOrDefaultAsync(o => o.Offer_ID == id && o.Recruitment_ID == recruiterId && o.Status == 0);

            if (shift == null)
                return RedirectWithError("Shift not found or cannot be cancelled");

            _context.OfferedShifts.Remove(shift);
            await _context.SaveChangesAsync();

            return RedirectWithSuccess($"Shift cancelled for {shift.Employee.User.First_Name}");
        }

        private async Task<List<RecruiterViewModel.StudentInfo>> GetStudents(int recruiterId)
        {
            return await _context.Employees
                .Where(e => e.Recruitment_ID == recruiterId)
                .Include(e => e.User)
                .Select(e => new RecruiterViewModel.StudentInfo
                {
                    StudentId = e.Student_ID,
                    Name = $"{e.User.First_Name} {e.User.Last_Name}",
                    Email = e.User.Email
                })
                .ToListAsync();
        }

        private async Task<List<RecruiterViewModel.StudentShiftRecord>> GetPendingStudentShifts(int recruiterId)
        {
            return await _context.OfferedShifts
                .Where(o => o.Status == 0 && o.Recruitment_ID == recruiterId)
                .Include(o => o.Employee.User)
                .Include(o => o.Employee.VisaStatuses)
                .Select(o => new RecruiterViewModel.StudentShiftRecord
                {
                    ShiftId = o.Offer_ID,
                    Date = o.Start_Date,
                    StudentId = o.Student_ID,
                    StudentName = $"{o.Employee.User.First_Name} {o.Employee.User.Last_Name}",
                    VisaStatus = o.Employee.VisaStatuses.Any(v => v.Status) ? "Required" : "Not Required",
                    StartTime = o.Start_Date,
                    EndTime = o.End_Date,
                    Duration = o.Total_Hours
                })
                .ToListAsync();
        }

        private async Task<List<RecruiterViewModel.AdminApprovalShiftRecord>> GetPendingAdminShifts(int recruiterId)
        {
            return await _context.OfferedShifts
                .Where(o => o.Status == 1 && o.Recruitment_ID == recruiterId)
                .Include(o => o.Employee.User)
                .Include(o => o.Employee.VisaStatuses)
                .Select(o => new RecruiterViewModel.AdminApprovalShiftRecord
                {
                    ShiftId = o.Offer_ID,
                    Date = o.Start_Date,
                    StudentId = o.Student_ID,
                    StudentName = $"{o.Employee.User.First_Name} {o.Employee.User.Last_Name}",
                    VisaStatus = o.Employee.VisaStatuses.Any(v => v.Status) ? "Required" : "Not Required",
                    StartTime = o.Start_Date,
                    EndTime = o.End_Date,
                    Duration = o.Total_Hours,
                    DateOffered = o.Date_Offered
                })
                .ToListAsync();
        }

        private async Task<List<RecruiterViewModel.ApprovedShiftRecord>> GetApprovedShifts(int recruiterId)
        {
            return await _context.OfferedShifts
                .Where(o => o.Status == 2 && o.Recruitment_ID == recruiterId)
                .Include(o => o.Employee.User)
                .Include(o => o.Employee.VisaStatuses)
                .Select(o => new RecruiterViewModel.ApprovedShiftRecord
                {
                    ShiftId = o.Offer_ID,
                    Date = o.Start_Date,
                    StudentId = o.Student_ID,
                    StudentName = $"{o.Employee.User.First_Name} {o.Employee.User.Last_Name}",
                    VisaStatus = o.Employee.VisaStatuses.Any(v => v.Status) ? "Required" : "Not Required",
                    StartTime = o.Start_Date,
                    EndTime = o.End_Date,
                    HoursWorked = o.Total_Hours,
                    DateApproved = o.Date_Offered
                })
                .ToListAsync();
        }

        private async Task<bool> ValidateStudent(int studentId, int recruiterId)
        {
            return await _context.Employees
                .AnyAsync(e => e.Student_ID == studentId && e.Recruitment_ID == recruiterId);
        }

        private IActionResult RedirectToLogin() => RedirectToAction("Index", "Login");
        
        private IActionResult RedirectWithError(string message)
        {
            TempData["Error"] = message;
            return RedirectToAction("Index");
        }
        
        private IActionResult RedirectWithSuccess(string message)
        {
            TempData["Success"] = message;
            return RedirectToAction("Index");
        }
    }
}
