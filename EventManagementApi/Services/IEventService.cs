using EventManagementApi.DTOs.Event;
using EventManagementApi.DTOs.Organizer;

namespace EventManagementApi.Services
{
    public interface IEventService
    {
        Task<List<EventGetDto>> GetAllAsync();
        Task<EventGetDto?> GetByIdAsync(int id);
        Task<List<EventGetDto>> GetByOrganizerIdAsync(int organizerId);
        Task<OrganizerGetDto?> GetOrganizerByEventIdAsync(int eventId);
        Task<string> CreateAsync(CreateEventDto dto);
        Task<string> UpdateAsync(int id, UpdateEventDto dto);
        Task<string> DeleteAsync(int id);
        Task<string> UploadBannerAsync(int id, IFormFile file, IWebHostEnvironment env);
    }
}
