using AutoMapper;
using EventManagementApi.DTOs.Event;
using EventManagementApi.DTOs.Organizer;
using EventManagementApi.Models;
using EventManagementApi.Repositories;

namespace EventManagementApi.Services
{
    public class EventService : IEventService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IMapper _mapper;

        public EventService(IEventRepository eventRepository, IOrganizerRepository organizerRepository, IMapper mapper)
        {
            _eventRepository = eventRepository;
            _organizerRepository = organizerRepository;
            _mapper = mapper;
        }

        public async Task<List<EventGetDto>> GetAllAsync()
        {
            var events = await _eventRepository.GetAllAsync();
            return _mapper.Map<List<EventGetDto>>(events);
        }

        public async Task<EventGetDto?> GetByIdAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null) return null;
            return _mapper.Map<EventGetDto>(ev);
        }

        public async Task<List<EventGetDto>> GetByOrganizerIdAsync(int organizerId)
        {
            var events = await _eventRepository.GetByOrganizerIdAsync(organizerId);
            return _mapper.Map<List<EventGetDto>>(events);
        }

        public async Task<OrganizerGetDto?> GetOrganizerByEventIdAsync(int eventId)
        {
            var ev = await _eventRepository.GetByIdAsync(eventId);
            if (ev == null) return null;
            return _mapper.Map<OrganizerGetDto>(ev.Organizer);
        }

        public async Task<string> CreateAsync(CreateEventDto dto)
        {
            var organizerExists = await _organizerRepository.GetByIdAsync(dto.OrganizerId);
            if (organizerExists == null)
                return "Bu ID-li organizer mövcud deyil!";

            var ev = _mapper.Map<Event>(dto);
            await _eventRepository.AddAsync(ev);
            return "Event uğurla əlavə edildi!";
        }

        public async Task<string> UpdateAsync(int id, UpdateEventDto dto)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
                return "Event tapılmadı!";

            var organizerExists = await _organizerRepository.GetByIdAsync(dto.OrganizerId);
            if (organizerExists == null)
                return "Bu ID-li organizer mövcud deyil!";

            _mapper.Map(dto, ev);
            await _eventRepository.UpdateAsync(ev);
            return "Event uğurla yeniləndi!";
        }

        public async Task<string> DeleteAsync(int id)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
                return "Event tapılmadı!";

            await _eventRepository.DeleteAsync(ev);
            return "Event uğurla silindi!";
        }

        public async Task<string> UploadBannerAsync(int id, IFormFile file, IWebHostEnvironment env)
        {
            var ev = await _eventRepository.GetByIdAsync(id);
            if (ev == null)
                return "Event tapılmadı!";

            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "banners");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            ev.BannerImageUrl = $"/uploads/banners/{fileName}";
            await _eventRepository.UpdateAsync(ev);
            return ev.BannerImageUrl;
        }
    }
}
