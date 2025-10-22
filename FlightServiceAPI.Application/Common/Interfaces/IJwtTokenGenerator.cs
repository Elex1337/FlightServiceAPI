using FlightServiceAPI.Domain.Entities;

namespace FlightServiceAPI.Application.Common.Interfaces;

public interface IJwtTokenGenerator
{
    string GenerateToken(User user);
}