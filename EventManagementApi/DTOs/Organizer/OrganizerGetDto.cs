namespace EventManagementApi.DTOs.Organizer
{
    public class OrganizerGetDto
    {
        public int OrganizerId { get; set; }
        public string Name { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? Phone { get; set; }
        public string? LogoUrl { get; set; }
    }
}
