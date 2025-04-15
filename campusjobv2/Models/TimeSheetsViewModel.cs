using System;
using System.Collections.Generic;

namespace campusjobv2.Models
{
    public class TimeSheetsViewModel
    {
        public List<ShiftInfo> ConfirmedShifts { get; set; } = new List<ShiftInfo>();
        public List<ShiftInfo> AvailableShifts { get; set; } = new List<ShiftInfo>();
        public List<ShiftInfo> PendingApprovalShifts { get; set; } = new List<ShiftInfo>();

        public class ShiftInfo
        {
            public int ShiftId { get; set; }
            public DateTime Date { get; set; }
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public decimal TotalHours { get; set; }
            public string Recruiter { get; set; }
            public string Status { get; set; } // "Available", "PendingApproval", or "Confirmed"
        }
    }
}
