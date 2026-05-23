using EventManagementApi.DTOs.Event;
using EventManagementApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class EventsController : ControllerBase
    {
        private readonly IEventService _eventService;
        private readonly IValidator<CreateEventDto> _createValidator;
        private readonly IValidator<UpdateEventDto> _updateValidator;
        private readonly IWebHostEnvironment _env;

        public EventsController(
            IEventService eventService,
            IValidator<CreateEventDto> createValidator,
            IValidator<UpdateEventDto> updateValidator,
            IWebHostEnvironment env)
        {
            _eventService = eventService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
            _env = env;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var events = await _eventService.GetAllAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var ev = await _eventService.GetByIdAsync(id);
            if (ev == null) return NotFound("Event tapılmadı!");
            return Ok(ev);
        }

        [HttpGet("{eventId}/organizer")]
        [AllowAnonymous]
        public async Task<IActionResult> GetOrganizer(int eventId)
        {
            var organizer = await _eventService.GetOrganizerByEventIdAsync(eventId);
            if (organizer == null) return NotFound("Event və ya organizer tapılmadı!");
            return Ok(organizer);
        }

        [HttpGet("{eventId}/tickets")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTickets(int eventId, [FromServices] ITicketService ticketService)
        {
            var tickets = await ticketService.GetByEventIdAsync(eventId);
            return Ok(tickets);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateEventDto dto)
        {
            var validation = await _createValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await _eventService.CreateAsync(dto);
            return Ok(result);
        }

        [HttpPost("{eventId}/tickets")]
        public async Task<IActionResult> CreateTicket(
            int eventId,
            [FromBody] DTOs.Ticket.CreateTicketDto dto,
            [FromServices] IValidator<DTOs.Ticket.CreateTicketDto> validator,
            [FromServices] ITicketService ticketService)
        {
            var validation = await validator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await ticketService.CreateAsync(eventId, dto);
            return Ok(result);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateEventDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(validation.Errors.Select(e => e.ErrorMessage));

            var result = await _eventService.UpdateAsync(id, dto);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _eventService.DeleteAsync(id);
            return Ok(result);
        }

        [HttpPost("{id}/banner")]
        public async Task<IActionResult> UploadBanner(int id, IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest("Fayl seçilməyib!");

            var result = await _eventService.UploadBannerAsync(id, file, _env);
            return Ok(new { BannerUrl = result });
        }
    }
}
