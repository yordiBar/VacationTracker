﻿namespace VacationTracker.Models
{
    public class Department
    {
        // Properties
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public int CompanyId { get; set; }
        public string DepartmentCode { get; set; }
        public bool IsDeleted { get; set; }
    }
}
