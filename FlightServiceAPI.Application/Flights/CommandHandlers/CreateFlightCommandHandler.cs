using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Commands;
using FlightServiceAPI.Domain.Entities;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Flights.CommandHandlers;

public class CreateFlightCommandHandler : IRequestHandler<CreateFlightCommand, Result<CreateFlightResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public CreateFlightCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<CreateFlightResponse>> Handle(CreateFlightCommand request, CancellationToken cancellationToken)
    {
        var flight = new Flight
        {
            Origin = request.Origin,
            Destination = request.Destination,
            Departure = request.Departure,
            Arrival = request.Arrival,
            Status = request.Status,
        };
        
        await _context.Flights.AddAsync(flight, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync("flights", cancellationToken);

        var response = new CreateFlightResponse(
            flight.Id,
            flight.Origin,
            flight.Destination,
            flight.Departure,
            flight.Arrival,
            flight.Status
        );

        return Result.Ok(response);
    }
}