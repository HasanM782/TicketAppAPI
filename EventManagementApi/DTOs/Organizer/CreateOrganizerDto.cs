namespace EventManagementApi.DTOs.Organizer
{
    public class CreateOrganizerDto
    {
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
    }
}
