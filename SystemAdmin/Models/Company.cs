using System;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace VacationTracker.SystemAdmin.Models
{
    public class Company
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Company name is required")]
        [StringLength(100, MinimumLength = 3, ErrorMessage = "Company name must be between 3 and 100 characters")]
        [RegularExpression(@"^[a-zA-Z0-9\s\-_\.]+$", ErrorMessage = "Company name can only contain letters, numbers, spaces, hyphens, underscores, and periods")]
        [Display(Name = "Company Name")]
        public string CompanyName { get; set; }

        public string DatabaseName { get; set; }
        public string ConnectionString { get; set; }
        public bool IsActive { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime? LastAccessed { get; set; }

        [Required(ErrorMessage = "Admin email is required")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address")]
        [Display(Name = "Admin Email")]
        public string AdminEmail { get; set; }

        public string AdminPassword { get; set; }
    }
}
