namespace EventManagementApi.DTOs.Auth
{
    public class ConfirmEmailDto
    {
        public string Email { get; set; } = null!;
        public string Code { get; set; } = null!;
    }
}