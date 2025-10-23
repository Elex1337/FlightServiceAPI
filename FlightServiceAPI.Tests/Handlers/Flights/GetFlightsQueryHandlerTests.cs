using FlightServiceAPI.Application.Common.Dtos.Flights;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Application.Flights.Queries;
using FlightServiceAPI.Application.Flights.QueryHandlers;
using FlightServiceAPI.Domain.Enums;
using FluentAssertions;
using Moq;
using Xunit;

namespace FlightServiceAPI.Tests.Handlers.Flights;

public class GetFlightsQueryHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<ICacheService> _cacheMock;
    private readonly GetFlightsQueryHandler _handler;

    public GetFlightsQueryHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _cacheMock = new Mock<ICacheService>();
        
        _handler = new GetFlightsQueryHandler(
            _contextMock.Object, 
            _cacheMock.Object);
    }

    [Fact]
    public async Task Handle_CacheHit_ReturnsCachedFlights()
    {
        var cachedFlights = new List<GetFlightsResponse>
        {
            new(1, "Almaty", "Astana", DateTimeOffset.Now, DateTimeOffset.Now.AddHours(1), Status.InTime)
        };

        _cacheMock
            .Setup(x => x.GetAsync<List<GetFlightsResponse>>("flights", It.IsAny<CancellationToken>()))
            .ReturnsAsync(cachedFlights);

        var query = new GetFlightsQuery(null, null);

        var result = await _handler.Handle(query, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Should().HaveCount(1);
        result.Value[0].Origin.Should().Be("Almaty");
    }
}