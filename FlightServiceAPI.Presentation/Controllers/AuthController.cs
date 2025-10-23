using FlightServiceAPI.Application.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FlightServiceAPI.Controllers;

[ApiController]
[Route("api/auth")]
public class AuthController : BaseController
{
    private readonly IMediator _mediator;

    public AuthController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return Unauthorized(new { message = result.Errors[0].Message });
        }
        return Ok(result.Value);
    }
}