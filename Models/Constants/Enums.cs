using System.ComponentModel;

namespace VacationTracker.Models.Constants
{
    public static class Enums
    {
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