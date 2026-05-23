using EventManagementApi.Data;
using EventManagementApi.DTOs.Auth;
using EventManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Services
{
    public class AuthService
    {
        private readonly AppDbContext _context;
        private readonly TokenService _tokenService;
        private readonly EmailService _emailService;

        public AuthService(AppDbContext context, TokenService tokenService, EmailService emailService)
        {
            _context = context;
            _tokenService = tokenService;
            _emailService = emailService;
        }

        public async Task<string> RegisterAsync(RegisterDto dto)
        {
            if (dto.Password != dto.ConfirmPassword)
                return "Şifrələr uyğun gəlmir!";

            var usernameExists = await _context.Users.AnyAsync(u => u.Username == dto.Username);
            if (usernameExists)
                return "Bu istifadəçi adı artıq mövcuddur!";

            var emailExists = await _context.Users.AnyAsync(u => u.Email == dto.Email);
            if (emailExists)
                return "Bu e-poçt artıq qeydiyyatdan keçib!";

            var user = new AppUser
            {
                Username = dto.Username,
                Email = dto.Email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password)
            };

            await _context.Users.AddAsync(user);
            await _context.SaveChangesAsync();

            return "Qeydiyyat uğurlu oldu!";
        }

        public async Task<AuthResponseDto?> LoginAsync(LoginDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Username == dto.Username);

            if (user == null) return null;

            var passwordCorrect = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!passwordCorrect) return null;

            var accessToken = _tokenService.CreateAccessToken(user);
            var refreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = accessToken,
                RefreshToken = refreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddDays(1),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<AuthResponseDto?> RefreshTokenAsync(RefreshTokenDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

            if (user == null) return null;

            if (user.RefreshTokenExpiry < DateTime.UtcNow) return null;

            var newAccessToken = _tokenService.CreateAccessToken(user);
            var newRefreshToken = _tokenService.CreateRefreshToken();

            user.RefreshToken = newRefreshToken;
            user.RefreshTokenExpiry = DateTime.UtcNow.AddDays(7);
            await _context.SaveChangesAsync();

            return new AuthResponseDto
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshToken,
                AccessTokenExpiry = DateTime.UtcNow.AddDays(1),
                RefreshTokenExpiry = DateTime.UtcNow.AddDays(7)
            };
        }

        public async Task<string> SendResetCodeAsync(ResetPasswordRequestDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return "Bu e-poçt ilə istifadəçi tapılmadı!";

            var code = _tokenService.GenerateResetCode();

            user.PasswordResetCode = code;
            user.PasswordResetCodeExpiry = DateTime.UtcNow.AddMinutes(10);
            await _context.SaveChangesAsync();

            await _emailService.SendResetCodeAsync(user.Email, code);

            return "Şifrə sıfırlama kodu e-poçtunuza göndərildi!";
        }

        public async Task<string> ResetPasswordAsync(ResetPasswordDto dto)
        {
            if (dto.NewPassword != dto.ConfirmNewPassword)
                return "Şifrələr uyğun gəlmir!";

            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.Email == dto.Email);

            if (user == null)
                return "İstifadəçi tapılmadı!";

            if (user.PasswordResetCode != dto.Code)
                return "Kod yanlışdır!";

            if (user.PasswordResetCodeExpiry < DateTime.UtcNow)
                return "Kodun vaxtı bitib! Yenidən sorğu edin.";

            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.NewPassword);
            user.PasswordResetCode = null;
            user.PasswordResetCodeExpiry = null;
            await _context.SaveChangesAsync();

            return "Şifrə uğurla yeniləndi!";
        }

        public async Task<string> LogoutAsync(RefreshTokenDto dto)
        {
            var user = await _context.Users
                .FirstOrDefaultAsync(u => u.RefreshToken == dto.RefreshToken);

            if (user == null)
                return "Token tapılmadı!";

            user.RefreshToken = null;
            user.RefreshTokenExpiry = null;
            await _context.SaveChangesAsync();

            return "Uğurla çıxış edildi!";
        }
    }
}
