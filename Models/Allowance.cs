using System;
using System.ComponentModel.DataAnnotations;

namespace VacationTracker.Models
{
    public class Allowance
    {
        // Properties
        public int Id { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }

        // Display DateTime as date only using DisplayFormat attribute
        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime To { get; set; }

        public int Amount { get; set; }
        public int CarryOver { get; set; }
        public int EmployeeId { get; set; }
        public int CompanyId { get; set; }
        public bool IsDeleted { get; set; }
    }
}
