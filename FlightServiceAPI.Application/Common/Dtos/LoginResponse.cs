namespace FlightServiceAPI.Application.Common.Dtos;

public record LoginResponse(string Token, string Username, string Role);