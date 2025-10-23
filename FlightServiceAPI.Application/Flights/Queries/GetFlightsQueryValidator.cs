using FluentValidation;

namespace FlightServiceAPI.Application.Flights.Queries;

public class GetFlightsQueryValidator : AbstractValidator<GetFlightsQuery>
{
    public GetFlightsQueryValidator()
    {
        When(x => !string.IsNullOrEmpty(x.Origin), () =>
        {
            RuleFor(x => x.Origin)
                .MaximumLength(256).WithMessage("Origin must not exceed 256 characters");
        });

        When(x => !string.IsNullOrEmpty(x.Destination), () =>
        {
            RuleFor(x => x.Destination)
                .MaximumLength(256).WithMessage("Destination must not exceed 256 characters");
        });
    }
}