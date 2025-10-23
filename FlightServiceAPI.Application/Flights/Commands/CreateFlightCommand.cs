using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Flights.Commands;

public record CreateFlightCommand(
    string Origin,
    string Destination,
    DateTimeOffset Departure,
    DateTimeOffset Arrival,
    Status Status) : IRequest<Result<CreateFlightResponse>>;