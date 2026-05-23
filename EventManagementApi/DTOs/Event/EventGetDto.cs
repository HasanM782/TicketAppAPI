namespace EventManagementApi.DTOs.Event
{
    public class EventGetDto
    {
        public int EventId { get; set; }
        public string Title { get; set; } = null!;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string Location { get; set; } = null!;
        public string? BannerImageUrl { get; set; }
        public int OrganizerId { get; set; }
        public string OrganizerName { get; set; } = null!;
    }
}
