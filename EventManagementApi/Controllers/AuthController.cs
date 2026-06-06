using EventManagementApi.Common;
using EventManagementApi.DTOs.Auth;
using EventManagementApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : ControllerBase
    {
        private readonly AuthService _authService;

        public AuthController(AuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterDto dto)
        {
            var result = await _authService.RegisterAsync(dto);
            return Ok(ApiResponse<object>.Ok(null, result));
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto dto)
        {
            var result = await _authService.LoginAsync(dto);
            if (result == null)
                return Unauthorized(ApiResponse<object>.Fail("İstifadəçi adı və ya şifrə yanlışdır!"));
            return Ok(ApiResponse<object>.Ok(result, "Uğurla daxil oldunuz!"));
        }

        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.RefreshTokenAsync(dto);
            if (result == null)
                return Unauthorized(ApiResponse<object>.Fail("Refresh token yanlış və ya vaxtı bitib!"));
            return Ok(ApiResponse<object>.Ok(result, "Token uğurla yeniləndi!"));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ResetPasswordRequestDto dto)
        {
            var result = await _authService.SendResetCodeAsync(dto);
            return Ok(ApiResponse<object>.Ok(null, result));
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDto dto)
        {
            var result = await _authService.ResetPasswordAsync(dto);
            return Ok(ApiResponse<object>.Ok(null, result));
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout([FromBody] RefreshTokenDto dto)
        {
            var result = await _authService.LogoutAsync(dto);
            if (result == "Token tapılmadı!")
                return NotFound(ApiResponse<object>.Fail(result));
            return Ok(ApiResponse<object>.Ok(null, result));
        }

        [HttpPost("confirm-email")]
        public async Task<IActionResult> ConfirmEmail([FromBody] ConfirmEmailDto dto)
        {
            var result = await _authService.ConfirmEmailAsync(dto);
            return Ok(ApiResponse<object>.Ok(null, result));
        }
    }
}