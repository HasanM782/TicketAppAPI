using EventManagementApi.DTOs.Organizer;

namespace EventManagementApi.Services
{
    public interface IOrganizerService
    {
        Task<List<OrganizerGetDto>> GetAllAsync();
        Task<OrganizerGetDto?> GetByIdAsync(int id);
        Task<string> CreateAsync(CreateOrganizerDto dto);
        Task<string> UpdateAsync(int id, UpdateOrganizerDto dto);
        Task<string> DeleteAsync(int id);
        Task<string> UploadLogoAsync(int id, IFormFile file, IWebHostEnvironment env);
    }
}
