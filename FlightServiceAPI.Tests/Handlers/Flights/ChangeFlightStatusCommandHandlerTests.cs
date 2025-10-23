using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Commands;
using FlightServiceAPI.Application.Flights.CommandHandlers;
using FlightServiceAPI.Domain.Entities;
using FlightServiceAPI.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace FlightServiceAPI.Tests.Handlers.Flights;

public class ChangeFlightStatusCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly ChangeFlightStatusCommandHandler _handler;

    public ChangeFlightStatusCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _cacheMock = new Mock<ICacheService>();
        
        _handler = new ChangeFlightStatusCommandHandler(
            _contextMock.Object, 
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ExistingFlight_UpdatesStatus()
    {
        var flight = new Flight
        {
            Id = 1,
            Origin = "Almaty",
            Destination = "Astana",
            Departure = DateTimeOffset.Now,
            Arrival = DateTimeOffset.Now.AddHours(1),
            Status = Status.InTime
        };

        _contextMock
            .Setup(x => x.Flights.FindAsync(1, It.IsAny<CancellationToken>()))
            .ReturnsAsync(flight);

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _cacheMock
            .Setup(x => x.RemoveAsync("flights", It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var command = new ChangeFlightStatusCommand(1, Status.Delayed);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Status.Should().Be(Status.Delayed);
        flight.Status.Should().Be(Status.Delayed);
    }
}