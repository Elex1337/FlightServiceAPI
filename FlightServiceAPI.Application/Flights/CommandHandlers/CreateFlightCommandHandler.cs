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
        await AddFlightToCache(response, cancellationToken);
        return Result.Ok(response);
    }
    private async Task AddFlightToCache(CreateFlightResponse newFlight, CancellationToken cancellationToken)
    {
        const string cacheKey = "flights";
        var flights = await _cache.GetAsync<List<GetFlightsResponse>>(cacheKey, cancellationToken);
        
        if (flights != null)
        {
            var flightResponse = new GetFlightsResponse(
                newFlight.Id,
                newFlight.Origin,
                newFlight.Destination,
                newFlight.Departure,
                newFlight.Arrival,
                newFlight.Status
            );
            
            flights.Add(flightResponse);
            
            flights = flights.OrderBy(f => f.Arrival).ToList();
            
            await _cache.SetAsync(cacheKey, flights, TimeSpan.FromMinutes(10), cancellationToken);
        }
    }
}