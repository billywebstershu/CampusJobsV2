using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace campusjobv2.Models
{
    public class AdminViewModel
    {
        public StudentAccountModel StudentAccount { get; set; } = new StudentAccountModel();
        public List<PendingShiftRecord> PendingShifts { get; set; } = new List<PendingShiftRecord>();
        public List<UserSearchResult> SearchResults { get; set; }
        public string SearchTerm { get; set; }

        public class PendingShiftRecord
        {
            public int ShiftId { get; set; }
            public int StudentId { get; set; }
            public string StudentName { get; set; }
            public string Recruiter { get; set; }
            public DateTime Date { get; set; }
            public decimal Hours { get; set; }
            public DateTime DateOffered { get; set; }
        }

        public class UserSearchResult
        {
            public int UserId { get; set; }
            public string FirstName { get; set; }
            public string LastName { get; set; }
            public string Email { get; set; }
            public string Role { get; set; }
            public bool HasDocuments { get; set; }
            public bool VisaStatus { get; set; }
        }
    }

    public class StudentAccountModel
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Department { get; set; }

        [Required]
        [Display(Name = "Recruiter")]
        public int RecruiterId { get; set; }

        public List<SelectListItem> AvailableRecruiters { get; set; } = new List<SelectListItem>();

        [Display(Name = "Visa Restricted")]
        public bool IsVisaRestricted { get; set; }

        [Display(Name = "Visa Expiry Date")]
        [RequiredIf("IsVisaRestricted", true, ErrorMessage = "Expiry date required for visa restricted students")]
        public DateTime? VisaExpiryDate { get; set; }
    }

    public class RequiredIfAttribute : ValidationAttribute
    {
        private string PropertyName { get; set; }
        private object DesiredValue { get; set; }

        public RequiredIfAttribute(string propertyName, object desiredValue, string errorMessage = "")
        {
            PropertyName = propertyName;
            DesiredValue = desiredValue;
            ErrorMessage = errorMessage;
        }

        protected override ValidationResult IsValid(object value, ValidationContext context)
        {
            var instance = context.ObjectInstance;
            var type = instance.GetType();
            var propertyValue = type.GetProperty(PropertyName)?.GetValue(instance, null);

            if (propertyValue?.ToString() == DesiredValue.ToString() && value == null)
            {
                return new ValidationResult(ErrorMessage);
            }
            return ValidationResult.Success;
        }
    }
}
