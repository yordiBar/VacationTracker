using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace VacationTracker.Models
{
    public class Location
    {
        // Properties
        public int Id { get; set; }
        public int CompanyId { get; set; }
        public string LocationCode { get; set; }
        public string LocationName { get; set; }
        public bool IsDeleted { get; set; }
        
        // Navigation Properties
        public virtual Company Company { get; set; }
    }
}
