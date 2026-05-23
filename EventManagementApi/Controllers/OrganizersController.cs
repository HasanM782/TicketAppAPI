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
            return Ok(organizers);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var organizer = await _organizerService.GetByIdAsync(id);
            if (organizer == null) return NotFound("Organizer tapılmadı!");
            return Ok(organizer);
        }

        [HttpGet("{organizerId}/events")]
        [AllowAnonymous]
        public async Task<IActionResult> GetEvents(int organizerId, [FromServices] IEventService eventService)
        {
            var events = await eventService.GetByOrganizerIdAsync(organizerId);
            return Ok(events);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateOrganizerDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await _organizerService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateOrganizerDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await _organizerService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _organizerService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/logo")]
        public async Task<IActionResult> UploadLogo(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fayl seçilməyib!");

            var result = await _organizerService.UploadLogoAsync(id, file, _env);
            return Ok(new { LogoUrl = result });
        }
    }
}
