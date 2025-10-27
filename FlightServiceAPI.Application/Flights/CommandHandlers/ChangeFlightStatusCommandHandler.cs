using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Commands;
using FlightServiceAPI.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Flights.CommandHandlers;

public class ChangeFlightStatusCommandHandler :  IRequestHandler<ChangeFlightStatusCommand, Result<ChangeFlightStatusResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;

    public ChangeFlightStatusCommandHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<ChangeFlightStatusResponse>> Handle(ChangeFlightStatusCommand request, CancellationToken cancellationToken)
    {
        var flight = await _context.Flights.FindAsync(request.Id);
        if (flight is null)
        {
            return Result.Fail<ChangeFlightStatusResponse>("Flight not found");
        }
        flight.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        await UpdateFlightStatusInCache(flight.Id,flight.Status,cancellationToken);
        
        var response = new ChangeFlightStatusResponse(flight.Id, flight.Status);

        return Result.Ok(response);
    }

    private async Task UpdateFlightStatusInCache(int flightId, Status newStatus, CancellationToken cancellationToken)
    {
        const string cacheKey = "flights";
        var flights = await _cache.GetAsync<List<GetFlightsResponse>>(cacheKey, cancellationToken);
        if (flights != null)
        {
            var flight = flights.SingleOrDefault(f => f.Id == flightId);
            if (flight != null)
            {
                var idx = flights.FindIndex(f => f.Id == flightId);
                flights[idx] = flight with { Status = newStatus };
                flights = flights.OrderBy(x => x.Arrival).ToList();
                await _cache.SetAsync(cacheKey, flights, TimeSpan.FromMinutes(10), cancellationToken);
            }
        }
    }
}
