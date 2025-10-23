using FluentValidation;

namespace FlightServiceAPI.Application.Flights.Commands;

public class ChangeFlightStatusCommandValidator : AbstractValidator<ChangeFlightStatusCommand>
{
    public ChangeFlightStatusCommandValidator()
    {
        RuleFor(x => x.Id)
            .NotEmpty().WithMessage("Id is required");
        RuleFor(x => x.Status)
            .IsInEnum().WithMessage("Status must be a valid flight status (InTime, Delayed, Cancelled)");
    }
}