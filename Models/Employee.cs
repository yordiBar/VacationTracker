using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace VacationTracker.Models
{
    public class Employee
    {
        public int Id { get; set; }
        public string Firstname { get; set; }
        public string Surname { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string EmployeeNumber { get; set; }
        public string JobTitle { get; set; }
        public bool IsDeleted { get; set; }
        public int CompanyId { get; set; }
        public int DepartmentId { get; set; }
        public int LocationId { get; set; }
        public int GenderId { get; set; }


        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime StartDate { get; set; }

        // Properties for diplay purposes, not migrated to the database
        [NotMapped]
        public string DisplayStartDate { get { return this.StartDate.ToString("dd/MM/yyyy"); } }

        // Password property to store Employee password, not migrated to the database
        [NotMapped]
        public string Password { get; set; }

        // Roles
        public bool IsApprover { get; set; }
        public bool IsManager { get; set; }
        public bool IsAdmin { get; set; }

        // Working days
        public bool Mon { get; set; }
        public bool Tue { get; set; }
        public bool Wed { get; set; }
        public bool Thu { get; set; }
        public bool Fri { get; set; }
        public bool Sat { get; set; }
        public bool Sun { get; set; }

        
    }
}
