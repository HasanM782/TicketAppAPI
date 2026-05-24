namespace EventManagementApi.DTOs.Auth
{
    public class ProfileUpdateDto
    {
        public string Username { get; set; } = null!;
        public string? FullName { get; set; }
    }
}   