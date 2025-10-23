using FlightServiceAPI.Application.Common.Dtos;
using FlightServiceAPI.Application.Common.Dtos.Auth;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponse>>;