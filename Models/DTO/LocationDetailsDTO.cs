namespace VacationTracker.Models.DTO
{
    public class LocationDetailsDTO
    {
        public int Id { get; set; }
        public string LocationName { get; set; }
        public string CompanyName { get; set; }
        public int? CompanyId { get; set; } = 0;
    }
}
