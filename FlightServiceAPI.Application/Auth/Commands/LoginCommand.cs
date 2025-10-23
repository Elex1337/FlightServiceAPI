using FlightServiceAPI.Application.Common.Dtos;
using FluentResults;
using MediatR;

namespace FlightServiceAPI.Application.Auth.Commands;

public record LoginCommand(string Username, string Password) : IRequest<Result<LoginResponse>>;