using AutoMapper;
using EventManagementApi.DTOs.Ticket;
using EventManagementApi.Models;
using EventManagementApi.Repositories;

namespace EventManagementApi.Services
{
    public class TicketService : ITicketService
    {
        private readonly ITicketRepository _ticketRepository;
        private readonly IEventRepository _eventRepository;
        private readonly IMapper _mapper;

        public TicketService(ITicketRepository ticketRepository, IEventRepository eventRepository, IMapper mapper)
        {
            _ticketRepository = ticketRepository;
            _eventRepository = eventRepository;
            _mapper = mapper;
        }

        public async Task<List<TicketGetDto>> GetAllAsync()
        {
            var tickets = await _ticketRepository.GetAllAsync();
            return _mapper.Map<List<TicketGetDto>>(tickets);
        }

        public async Task<List<TicketGetDto>> GetByEventIdAsync(int eventId)
        {
            var tickets = await _ticketRepository.GetByEventIdAsync(eventId);
            return _mapper.Map<List<TicketGetDto>>(tickets);
        }

        public async Task<TicketGetDto?> GetByIdAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null) return null;
            return _mapper.Map<TicketGetDto>(ticket);
        }

        public async Task<string> CreateAsync(int eventId, CreateTicketDto dto)
        {
            var eventExists = await _eventRepository.GetByIdAsync(eventId);
            if (eventExists == null)
                return "Bu ID-li event mövcud deyil!";

            var ticket = _mapper.Map<Ticket>(dto);
            ticket.EventId = eventId;
            await _ticketRepository.AddAsync(ticket);
            return "Bilet uğurla əlavə edildi!";
        }

        public async Task<string> UpdateAsync(int id, UpdateTicketDto dto)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                return "Bilet tapılmadı!";

            _mapper.Map(dto, ticket);
            await _ticketRepository.UpdateAsync(ticket);
            return "Bilet uğurla yeniləndi!";
        }

        public async Task<string> DeleteAsync(int id)
        {
            var ticket = await _ticketRepository.GetByIdAsync(id);
            if (ticket == null)
                return "Bilet tapılmadı!";

            await _ticketRepository.DeleteAsync(ticket);
            return "Bilet uğurla silindi!";
        }
    }
}
