namespace EventManagementApi.Models
{
    public class AppUser
    {
        public int Id { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;

        // Refresh Token
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }

        // Reset Password
        public string? PasswordResetCode { get; set; }
        public DateTime? PasswordResetCodeExpiry { get; set; }
        // Email Confirmation
        public bool IsEmailConfirmed { get; set; } = false;
        public string? EmailConfirmationCode { get; set; }
        public DateTime? EmailConfirmationCodeExpiry { get; set; }

        public string? FullName { get; set; }
        public string Role { get; set; } = "User"; // Default: "User", digəri: "Admin"
    }
}
