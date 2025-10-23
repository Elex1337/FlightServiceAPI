using FlightServiceAPI.Domain.Enums;

namespace FlightServiceAPI.Application.Common.Dtos.Flights;

public record ChangeFlightStatusResponse(int Id, Status Status);
