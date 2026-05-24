using EventManagementApi.Data;
using EventManagementApi.DTOs.Auth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class ProfileController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ProfileController(AppDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> GetProfile()
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("İstifadəçi tapılmadı!");

            return Ok(new ProfileGetDto
            {
                Id = user.Id,
                Username = user.Username,
                Email = user.Email,
                FullName = user.FullName,
                Role = user.Role
            });
        }

        [HttpPut]
        public async Task<IActionResult> UpdateProfile([FromBody] ProfileUpdateDto dto)
        {
            var userId = int.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("İstifadəçi tapılmadı!");

            var usernameExists = await _context.Users
                .AnyAsync(u => u.Username == dto.Username && u.Id != userId);
            if (usernameExists) return BadRequest("Bu username artıq istifadə olunur!");

            user.Username = dto.Username;
            user.FullName = dto.FullName;
            await _context.SaveChangesAsync();
            return Ok("Profil uğurla yeniləndi!");
        }

        [HttpPut("role/{userId}")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> ChangeRole(int userId, [FromBody] string newRole)
        {
            if (newRole != "Admin" && newRole != "User")
                return BadRequest("Rol yalnız 'Admin' və ya 'User' ola bilər!");

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == userId);
            if (user == null) return NotFound("İstifadəçi tapılmadı!");

            user.Role = newRole;
            await _context.SaveChangesAsync();
            return Ok($"İstifadəçinin rolu '{newRole}' olaraq dəyişdirildi!");
        }
    }
}