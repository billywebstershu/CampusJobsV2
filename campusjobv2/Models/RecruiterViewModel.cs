using System;
using System.Collections.Generic;

namespace campusjobv2.Models
{
    public class RecruiterViewModel
    {
        public List<StudentShiftRecord> PendingStudentApprovalShifts { get; set; } = new List<StudentShiftRecord>();
        public List<AdminApprovalShiftRecord> PendingAdminApprovalShifts { get; set; } = new List<AdminApprovalShiftRecord>();
        public List<ApprovedShiftRecord> ApprovedShifts { get; set; } = new List<ApprovedShiftRecord>();
        public List<StudentInfo> AvailableStudents { get; set; } = new List<StudentInfo>();

        public class StudentShiftRecord
        {
            public int ShiftId { get; set; }
            public DateTime Date { get; set; }
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public string VisaStatus { get; set; } // Display only
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public decimal Duration { get; set; }
        }

        public class AdminApprovalShiftRecord
        {
            public int ShiftId { get; set; }
            public DateTime Date { get; set; }
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public string VisaStatus { get; set; } // Display only
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public decimal Duration { get; set; }
            public DateTime DateOffered { get; set; }
        }

        public class ApprovedShiftRecord
        {
            public int ShiftId { get; set; }
            public DateTime Date { get; set; }
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public string VisaStatus { get; set; } // Display only
            public DateTime StartTime { get; set; }
            public DateTime EndTime { get; set; }
            public decimal HoursWorked { get; set; }
            public DateTime DateApproved { get; set; }
        }

        public class StudentInfo
        {
            public int StudentId { get; set; }
            public string Name { get; set; }
            public string Email { get; set; }
        }
    }
}
