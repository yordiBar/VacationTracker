namespace VacationTracker.Models.DTO
{
    public class DepartmentDetailsDTO
    {
        public int Id { get; set; }
        public string DepartmentName { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; } = 0;
    }
}
