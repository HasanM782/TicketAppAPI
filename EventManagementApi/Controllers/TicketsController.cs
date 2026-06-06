using EventManagementApi.Common;
using EventManagementApi.DTOs.Ticket;
using EventManagementApi.Services;
using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EventManagementApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [Authorize]
    public class TicketsController : ControllerBase
    {
        private readonly ITicketService _ticketService;
        private readonly IValidator<CreateTicketDto> _createValidator;
        private readonly IValidator<UpdateTicketDto> _updateValidator;

        public TicketsController(
            ITicketService ticketService,
            IValidator<CreateTicketDto> createValidator,
            IValidator<UpdateTicketDto> updateValidator)
        {
            _ticketService = ticketService;
            _createValidator = createValidator;
            _updateValidator = updateValidator;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAll()
        {
            var tickets = await _ticketService.GetAllAsync();
            return Ok(ApiResponse<object>.Ok(tickets));
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetById(int id)
        {
            var ticket = await _ticketService.GetByIdAsync(id);
            if (ticket == null)
                return NotFound(ApiResponse<object>.Fail("Bilet tapılmadı!"));
            return Ok(ApiResponse<object>.Ok(ticket));
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] UpdateTicketDto dto)
        {
            var validation = await _updateValidator.ValidateAsync(dto);
            if (!validation.IsValid)
                return BadRequest(ApiResponse<object>.Fail("Validation xətası!", validation.Errors.Select(e => e.ErrorMessage).ToList()));

            var result = await _ticketService.UpdateAsync(id, dto);
            return Ok(ApiResponse<object>.Ok(result, "Bilet uğurla yeniləndi!"));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var result = await _ticketService.DeleteAsync(id);
            return Ok(ApiResponse<object>.Ok(null, "Bilet uğurla silindi!"));
        }
    }
}