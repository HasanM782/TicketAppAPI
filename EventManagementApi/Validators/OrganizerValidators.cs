using EventManagementApi.DTOs.Organizer;
using FluentValidation;

namespace EventManagementApi.Validators
{
    public class CreateOrganizerDtoValidator : AbstractValidator<CreateOrganizerDto>
    {
        public CreateOrganizerDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
                .When(x => x.Phone != null);
        }
    }

    public class UpdateOrganizerDtoValidator : AbstractValidator<UpdateOrganizerDto>
    {
        public UpdateOrganizerDtoValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(100).WithMessage("Name must not exceed 100 characters.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email address is required.");

            RuleFor(x => x.Phone)
                .MaximumLength(20).WithMessage("Phone must not exceed 20 characters.")
                .When(x => x.Phone != null);
        }
    }
}
