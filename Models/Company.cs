﻿namespace VacationTracker.Models
{
    public class Company
    {
        // Properties
        public int Id { get; set; }
        public string CompanyName { get; set; }
        public string Address { get; set; }
        public string PhoneNumber { get; set; }
        public string ContactEmail { get; set; }
        public string ContactName { get; set; }
        public bool IsDeleted { get; set; }
    }
}
