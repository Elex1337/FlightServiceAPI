using FlightServiceAPI.Application.Flights.Commands;
using FlightServiceAPI.Application.Flights.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FlightServiceAPI.Controllers;
[ApiController]
[Route("api/flights")]
public class FlightController : BaseController
{
    private readonly IMediator  _mediator;

    public FlightController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost("create")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> CreateFlight([FromBody] CreateFlightCommand command)
    {
        var result = await _mediator.Send(command);
        if (result.IsFailed)
        {
            return BadRequest( new { message = result.Errors[0].Message });
        }
        return Created("flight", result.Value);
    }
    
    [HttpPatch("update-status")]
    [Authorize(Roles = "Moderator")]
    public async Task<IActionResult> ChangeFlightStatus([FromBody] ChangeFlightStatusCommand command)
    {
        var result  = await _mediator.Send(command);
        
        if (result.IsFailed)
        {
            return BadRequest(new { message = result.Errors[0].Message });
        }
        
        return Ok(result.Value);
    }

    [HttpGet]
    [Authorize]
    public async Task<IActionResult> GetFlights([FromQuery] GetFlightsQuery query)
    {
        var result = await _mediator.Send(query);
        if (result.IsFailed)
        {
            return NotFound(new { message = result.Errors[0].Message });
        }
        return Ok(result.Value);
    }
    
}