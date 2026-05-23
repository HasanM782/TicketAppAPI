using EventManagementApi.DTOs.Ticket;

namespace EventManagementApi.Services
{
    public interface ITicketService
    {
        Task<List<TicketGetDto>> GetAllAsync();
        Task<List<TicketGetDto>> GetByEventIdAsync(int eventId);
        Task<TicketGetDto?> GetByIdAsync(int id);
        Task<string> CreateAsync(int eventId, CreateTicketDto dto);
        Task<string> UpdateAsync(int id, UpdateTicketDto dto);
        Task<string> DeleteAsync(int id);
    }
}
