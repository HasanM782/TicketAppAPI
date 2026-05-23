using EventManagementApi.DTOs.Ticket;
using FluentValidation;

namespace EventManagementApi.Validators
{
    public class CreateTicketDtoValidator : AbstractValidator<CreateTicketDto>
    {
        public CreateTicketDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Ticket type is required.")
                .MaximumLength(50).WithMessage("Type must not exceed 50 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be a positive value.");

            RuleFor(x => x.QuantityAvailable)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be zero or more.");
        }
    }

    public class UpdateTicketDtoValidator : AbstractValidator<UpdateTicketDto>
    {
        public UpdateTicketDtoValidator()
        {
            RuleFor(x => x.Type)
                .NotEmpty().WithMessage("Ticket type is required.")
                .MaximumLength(50).WithMessage("Type must not exceed 50 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be a positive value.");

            RuleFor(x => x.QuantityAvailable)
                .GreaterThanOrEqualTo(0).WithMessage("Quantity must be zero or more.");
        }
    }
}
