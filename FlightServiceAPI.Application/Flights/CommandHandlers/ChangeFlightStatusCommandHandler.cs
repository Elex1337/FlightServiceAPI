using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Commands;
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
        var flight = await _context.Flights.FindAsync(request.Id, cancellationToken);
        if (flight is null)
        {
            return Result.Fail<ChangeFlightStatusResponse>("Flight not found");
        }
        flight.Status = request.Status;

        await _context.SaveChangesAsync(cancellationToken);

        await _cache.RemoveAsync("flights", cancellationToken);

        var response = new ChangeFlightStatusResponse(flight.Id, flight.Status);

        return Result.Ok(response);
    }
}