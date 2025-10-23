using FlightServiceAPI.Domain.Enums;

namespace FlightServiceAPI.Application.Common.Dtos.Flights;

public record CreateFlightResponse(
    int Id,
    string Origin,
    string Destination,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    Status Status);