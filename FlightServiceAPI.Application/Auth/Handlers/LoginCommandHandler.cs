using FlightServiceAPI.Application.Auth.Commands;
using FlightServiceAPI.Application.Common.Dtos;
using FlightServiceAPI.Application.Common.Interfaces;
using FluentResults;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace FlightServiceAPI.Application.Auth.Handlers;

public class LoginCommandHandler : IRequestHandler<LoginCommand, Result<LoginResponse>>
{
    private readonly IApplicationDbContext _context;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenGenerator  _jwtTokenGenerator;
    
    public LoginCommandHandler(IPasswordHasher passwordHasher, IJwtTokenGenerator jwtTokenGenerator, IApplicationDbContext context)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _jwtTokenGenerator = jwtTokenGenerator;
    }

    public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user =  await _context.Users.AsNoTracking().FirstOrDefaultAsync(x => x.Username == request.Username, cancellationToken: cancellationToken);
        
        if (user is null)
        {
            return Result.Fail<LoginResponse>("Invalid username or password");
        }
        
        bool verifyPassword = _passwordHasher.VerifyPassword(request.Password, user.Password);
        
        if (!verifyPassword)
        {
            return Result.Fail<LoginResponse>("Invalid username or password");
        }

        var token = _jwtTokenGenerator.GenerateToken(user);
        
        var response = new LoginResponse(
            token,
            user.Username,
            user?.Role?.Code ?? "User"
        );
        
        return Result.Ok(response);
    }
}