using EventManagementApi.Common;
using EventManagementApi.DTOs.Organizer;
using EventManagementApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class OrganizersController : ControllerBase
    {
        private readonly IOrganizerService _organizerService;
        private readonly IValidator<CreateOrganizerDto> _createValidator;
        private readonly IValidator<UpdateOrganizerDto> _updateValidator;
        private readonly IWebHostEnvironment _env;

        public OrganizersController(
            IOrganizerService organizerService,
            IValidator<CreateOrganizerDto> createValidator,
            IValidator<UpdateOrganizerDto> updateValidator,
            IWebHostEnvironment env)
        {
            _organizerService = organizerService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var organizers = await _organizerService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(organizers));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var organizer = await _organizerService.GetByIdAsync(id);
            if (organizer == null)
                return NotFound(ApiResponse<object>.NotFound("Organizer tapılmadı!"));
            return Ok(ApiResponse<object>.Ok(organizer));
        }

        [HttpGet("{organizerId}/events")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents(int organizerId, [FromServices] IEventService eventService)
        {
            var events = await eventService.GetByOrganizerIdAsync(organizerId);
            return Ok(ApiResponse<object>.Ok(events));
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrganizerDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse<object>.BadRequest(errors));
            }
            var result = await _organizerService.CreateAsync(dto);
            return Ok(ApiResponse<object>.Ok(result, "Organizer uğurla yaradıldı!"));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizerDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
            {
                var errors = string.Join(", ", validation.Errors.Select(e => e.ErrorMessage));
                return BadRequest(ApiResponse<object>.BadRequest(errors));
            }
            var result = await _organizerService.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.Ok(result, "Organizer uğurla yeniləndi!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _organizerService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Organizer uğurla silindi!"));
        }

        [HttpPost("{id}/logo")]
        public async Task<IActionResult> UploadLogo(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest(ApiResponse<object>.BadRequest("Fayl seçilməyib!"));
            var result = await _organizerService.UploadLogoAsync(id, file, _env);
            return Ok(ApiResponse<object>.Ok(new { LogoUrl = result }, "Logo uğurla yükləndi!"));
        }
    }
}