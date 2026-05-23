using AutoMapper;
using EventManagementApi.DTOs.Organizer;
using EventManagementApi.Models;
using EventManagementApi.Repositories;

namespace EventManagementApi.Services
{
    public class OrganizerService : IOrganizerService
    {
        private readonly IOrganizerRepository _organizerRepository;
        private readonly IMapper _mapper;

        public OrganizerService(IOrganizerRepository organizerRepository, IMapper mapper)
        {
            _organizerRepository = organizerRepository;
            _mapper = mapper;
        }

        public async Task<List<OrganizerGetDto>> GetAllAsync()
        {
            var organizers = await _organizerRepository.GetAllAsync();
            return _mapper.Map<List<OrganizerGetDto>>(organizers);
        }

        public async Task<OrganizerGetDto?> GetByIdAsync(int id)
        {
            var organizer = await _organizerRepository.GetByIdAsync(id);
            if (organizer == null) return null;
            return _mapper.Map<OrganizerGetDto>(organizer);
        }

        public async Task<string> CreateAsync(CreateOrganizerDto dto)
        {
            var emailExists = await _organizerRepository.EmailExistsAsync(dto.Email);
            if (emailExists)
                return "Bu e-poçt artıq istifadə olunur!";

            var organizer = _mapper.Map<Organizer>(dto);
            await _organizerRepository.AddAsync(organizer);
            return "Organizer uğurla əlavə edildi!";
        }

        public async Task<string> UpdateAsync(int id, UpdateOrganizerDto dto)
        {
            var organizer = await _organizerRepository.GetByIdAsync(id);
            if (organizer == null)
                return "Organizer tapılmadı!";

            _mapper.Map(dto, organizer);
            await _organizerRepository.UpdateAsync(organizer);
            return "Organizer uğurla yeniləndi!";
        }

        public async Task<string> DeleteAsync(int id)
        {
            var organizer = await _organizerRepository.GetByIdAsync(id);
            if (organizer == null)
                return "Organizer tapılmadı!";

            await _organizerRepository.DeleteAsync(organizer);
            return "Organizer uğurla silindi!";
        }

        public async Task<string> UploadLogoAsync(int id, IFormFile file, IWebHostEnvironment env)
        {
            var organizer = await _organizerRepository.GetByIdAsync(id);
            if (organizer == null)
                return "Organizer tapılmadı!";

            var uploadsFolder = Path.Combine(env.WebRootPath, "uploads", "logos");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            organizer.LogoUrl = $"/uploads/logos/{fileName}";
            await _organizerRepository.UpdateAsync(organizer);
            return organizer.LogoUrl;
        }
    }
}
