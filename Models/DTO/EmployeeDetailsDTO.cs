using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace VacationTracker.Models.DTO
{
    public class EmployeeDetailsDTO
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; } = 0;
        public string DepartmentName { get; set; }
        public string LocationName { get; set; }
        public string GenderName { get; set; }
        public DateTime StartDate { get; set; }
        public string JobTitle { get; set; }
        public bool IsAdmin { get; set; }
        public bool IsApprover { get; set; }
        public bool IsManager { get; set; }
        public string DisplayStartDate { get { return this.StartDate.ToString("dd/MM/yyyy"); } }
    }
}
