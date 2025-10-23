using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Domain.Enums;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Flights.Commands;

public record ChangeFlightStatusCommand(int Id, Status Status) : IRequest<Result<ChangeFlightStatusResponse>>;