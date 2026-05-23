namespace EventManagementApi.DTOs.Organizer
{
    public class UpdateOrganizerDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
