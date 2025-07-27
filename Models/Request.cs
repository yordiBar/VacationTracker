using System;
using System.ComponentModel.DataAnnotations;

namespace VacationTracker.Models
{
    public class Request
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public int RequestTypeId { get; set; }
        public int EmployeeId { get; set; }
        public double RequestAmount { get; set; }
        public string Description { get; set; }
        public int Status { get; set; }
        public int RequestCreatedByEmployeeId { get; set; }
        public bool IsActive { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime To { get; set; }
    }
}
