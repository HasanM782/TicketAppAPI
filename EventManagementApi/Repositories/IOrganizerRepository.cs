using EventManagementApi.Models;

namespace EventManagementApi.Repositories
{
    public interface IOrganizerRepository
    {
        Task<List<Organizer>> GetAllAsync();
        Task<Organizer?> GetByIdAsync(int id);
        Task<bool> EmailExistsAsync(string email);
        Task AddAsync(Organizer organizer);
        Task UpdateAsync(Organizer organizer);
        Task DeleteAsync(Organizer organizer);
    }
}
