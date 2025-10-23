using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Queries;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightServiceAPI.Application.Flights.QueryHandlers;

public class GetFlightsQueryHandler : IRequestHandler<GetFlightsQuery, Result<List<GetFlightsResponse?>>>
{
    private readonly IApplicationDbContext _context;
    private readonly ICacheService _cache;
    
    public GetFlightsQueryHandler(IApplicationDbContext context, ICacheService cache)
    {
        _context = context;
        _cache = cache;
    }

    public async Task<Result<List<GetFlightsResponse?>>> Handle(GetFlightsQuery request, CancellationToken cancellationToken)
    {
        const string cacheKey = "flights";
        var cached = await _cache.GetAsync<List<GetFlightsResponse>>(cacheKey, cancellationToken);
        
        if (cached != null)
        {
            var filteredCached = FilterFlights(cached, request.Origin, request.Destination);
            return Result.Ok(filteredCached);
        }
        
        var flights = await _context.Flights
            .AsNoTracking()
            .OrderBy(f => f.Arrival) 
            .Select(f => new GetFlightsResponse(
                f.Id,
                f.Origin,
                f.Destination,
                f.Departure,
                f.Arrival,
                f.Status
            ))
            .ToListAsync(cancellationToken);
        
        await _cache.SetAsync(cacheKey, flights, TimeSpan.FromMinutes(10), cancellationToken);
        var filtered = FilterFlights(flights, request.Origin, request.Destination);
        return Result.Ok(filtered);
    }
    private static List<GetFlightsResponse> FilterFlights(List<GetFlightsResponse> flights, string? origin, string? destination)
    {
        var query = flights.AsQueryable();

        if (!string.IsNullOrEmpty(origin))
        {
            query = query.Where(f => f.Origin.Equals(origin, StringComparison.OrdinalIgnoreCase));
        }

        if (!string.IsNullOrEmpty(destination))
        {
            query = query.Where(f => f.Destination.Equals(destination, StringComparison.OrdinalIgnoreCase));
        }

        return query.ToList();
    }
}