using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Commands;
using FlightServiceAPI.Application.Flights.CommandHandlers;
using FlightServiceAPI.Domain.Entities;
using FlightServiceAPI.Domain.Enums;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Moq;
using Xunit;

namespace FlightServiceAPI.Tests.Handlers.Flights;

public class CreateFlightCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly Mock<DbSet<Flight>> _flightsDbSetMock;
    private readonly CreateFlightCommandHandler _handler;

    public CreateFlightCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _cacheMock = new Mock<ICacheService>();
        _flightsDbSetMock = new Mock<DbSet<Flight>>();
        
        _contextMock.Setup(x => x.Flights).Returns(_flightsDbSetMock.Object);
        
        _handler = new CreateFlightCommandHandler(
            _contextMock.Object, 
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCommand_CreatesFlightSuccessfully()
    {
        _flightsDbSetMock
            .Setup(x => x.AddAsync(It.IsAny<Flight>(), It.IsAny<CancellationToken>()))
            .Callback<Flight, CancellationToken>((f, ct) => f.Id = 1)
            .Returns(new ValueTask<EntityEntry<Flight>>());

        _contextMock
            .Setup(x => x.SaveChangesAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(1);

        _cacheMock
            .Setup(x => x.RemoveAsync("flights", It.IsAny<CancellationToken>()))
            .Returns(ValueTask.CompletedTask);

        var command = new CreateFlightCommand(
            "Almaty",
            "Astana",
            DateTimeOffset.Now,
            DateTimeOffset.Now.AddHours(1),
            Status.InTime);

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Origin.Should().Be("Almaty");
        result.Value.Destination.Should().Be("Astana");
    }
}