using System;
using System.Collections.Generic;
using System.ComponentModel;
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
        public string Status { get; set; }
        public int RequestCreatedByEmployeeId { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime From { get; set; }

        [DisplayFormat(DataFormatString = "{0:d}", ApplyFormatInEditMode = true)]
        public DateTime To { get; set; }
        public enum RequestStatus
        {
            [Description("Pending")]
            Pending = 0,
            [Description("Approved")]
            Approved = 1,
            [Description("Taken")]
            Taken = 2,
            [Description("Cancelled")]
            Cancelled = 3,
            [Description("Rejected")]
            Rejected = 4
        }
    }
}
