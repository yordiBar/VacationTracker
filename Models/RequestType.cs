using System.Collections.Generic;

namespace VacationTracker.Models
{
    public class RequestType
    {
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string RequesTypeCode { get; set; }
        public string RequestTypeName { get; set; }
        public string Description { get; set; }
        public bool IsDeleted { get; set; }
        public bool TakesFromAllowance { get; set; }
    }
}
