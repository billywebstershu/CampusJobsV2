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
    public class TimeSheetsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TimeSheetsController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null) return RedirectToAction("Index", "Login");

            var model = new TimeSheetsViewModel
            {
                ConfirmedShifts = await GetConfirmedShifts(studentId.Value),
                AvailableShifts = await GetAvailableShifts(studentId.Value),
                PendingApprovalShifts = await GetPendingApprovalShifts(studentId.Value)
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> AcceptShift(int shiftId)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null) return RedirectToAction("Index", "Login");

            var shift = await _context.OfferedShifts
                .FirstOrDefaultAsync(o => o.Offer_ID == shiftId && o.Student_ID == studentId && o.Status == 0);

            if (shift == null)
                return RedirectWithError("Shift not found or already accepted");

            shift.Status = 1;
            await _context.SaveChangesAsync();

            return RedirectWithSuccess("Shift accepted! Waiting for admin approval.");
        }

        [HttpPost]
        public async Task<IActionResult> DeclineShift(int shiftId)
        {
            var studentId = HttpContext.Session.GetInt32("StudentId");
            if (studentId == null) return RedirectToAction("Index", "Login");

            var shift = await _context.OfferedShifts
                .FirstOrDefaultAsync(o => o.Offer_ID == shiftId && o.Student_ID == studentId && o.Status == 0);

            if (shift == null)
                return RedirectWithError("Cannot decline - shift not found or already accepted");

            _context.OfferedShifts.Remove(shift);
            await _context.SaveChangesAsync();

            return RedirectWithSuccess("Shift declined successfully");
        }

        private async Task<List<TimeSheetsViewModel.ShiftInfo>> GetConfirmedShifts(int studentId)
        {
            return await _context.OfferedShifts
                .Include(o => o.Recruiter.User)
                .Where(o => o.Student_ID == studentId && o.Status == 2)
                .Select(o => new TimeSheetsViewModel.ShiftInfo
                {
                    ShiftId = o.Offer_ID,
                    Date = o.Start_Date,
                    StartTime = o.Start_Date,
                    EndTime = o.End_Date,
                    TotalHours = o.Total_Hours,
                    Recruiter = $"{o.Recruiter.User.First_Name} {o.Recruiter.User.Last_Name}",
                    Status = "Confirmed"
                })
                .OrderBy(s => s.Date)
                .ToListAsync();
        }

        private async Task<List<TimeSheetsViewModel.ShiftInfo>> GetAvailableShifts(int studentId)
        {
            return await _context.OfferedShifts
                .Include(o => o.Recruiter.User)
                .Where(o => o.Student_ID == studentId && o.Status == 0)
                .Select(o => new TimeSheetsViewModel.ShiftInfo
                {
                    ShiftId = o.Offer_ID,
                    Date = o.Start_Date,
                    StartTime = o.Start_Date,
                    EndTime = o.End_Date,
                    TotalHours = o.Total_Hours,
                    Recruiter = $"{o.Recruiter.User.First_Name} {o.Recruiter.User.Last_Name}",
                    Status = "Available"
                })
                .ToListAsync();
        }

        private async Task<List<TimeSheetsViewModel.ShiftInfo>> GetPendingApprovalShifts(int studentId)
        {
            return await _context.OfferedShifts
                .Include(o => o.Recruiter.User)
                .Where(o => o.Student_ID == studentId && o.Status == 1)
                .Select(o => new TimeSheetsViewModel.ShiftInfo
                {
                    ShiftId = o.Offer_ID,
                    Date = o.Start_Date,
                    StartTime = o.Start_Date,
                    EndTime = o.End_Date,
                    TotalHours = o.Total_Hours,
                    Recruiter = $"{o.Recruiter.User.First_Name} {o.Recruiter.User.Last_Name}",
                    Status = "PendingApproval"
                })
                .ToListAsync();
        }

public async Task<IActionResult> GetNotifications()
{
    var studentId = HttpContext.Session.GetInt32("StudentId");
    if (studentId == null) return RedirectToAction("Index", "Login");

    var notifications = await _context.Notifications
        .Where(n => n.User_ID == studentId)
        .OrderByDescending(n => n.Time)
        .ToListAsync();

    return Json(notifications);
}

[HttpPost]
public async Task<IActionResult> MarkAsRead(int notificationId)
{
    var notification = await _context.Notifications.FindAsync(notificationId);
    if (notification != null)
    {
        notification.Read = true;
        await _context.SaveChangesAsync();
    }
    return Ok();
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

