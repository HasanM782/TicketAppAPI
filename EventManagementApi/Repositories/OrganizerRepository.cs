using EventManagementApi.Data;
using EventManagementApi.Models;
using Microsoft.EntityFrameworkCore;

namespace EventManagementApi.Repositories
{
    public class OrganizerRepository : IOrganizerRepository
    {
        private readonly AppDbContext _context;

        public OrganizerRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<List<Organizer>> GetAllAsync()
        {
            return await _context.Organizers.ToListAsync();
        }

        public async Task<Organizer?> GetByIdAsync(int id)
        {
            return await _context.Organizers.FirstOrDefaultAsync(o => o.OrganizerId == id);
        }

        public async Task<bool> EmailExistsAsync(string email)
        {
            return await _context.Organizers.AnyAsync(o => o.Email == email);
        }

        public async Task AddAsync(Organizer organizer)
        {
            await _context.Organizers.AddAsync(organizer);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Organizer organizer)
        {
            _context.Organizers.Update(organizer);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Organizer organizer)
        {
            _context.Organizers.Remove(organizer);
            await _context.SaveChangesAsync();
        }
    }
}
