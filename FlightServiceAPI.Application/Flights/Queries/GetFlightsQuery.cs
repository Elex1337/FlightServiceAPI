using FlightServiceAPI.Application.Common.Dtos.Flights;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Flights.Queries;

public record GetFlightsQuery(string? Origin, string Destination) : IRequest<Result<List<GetFlightsResponse?>>>; 