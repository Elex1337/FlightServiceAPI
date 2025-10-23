using FlightServiceAPI.Application.Auth.Commands;
using FlightServiceAPI.Application.Auth.Handlers;
using FlightServiceAPI.Application.Common.Interfaces;
using FlightServiceAPI.Domain.Entities;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace FlightServiceAPI.Tests.Handlers.Auth;

public class LoginCommandHandlerTests
{
    private readonly Mock<IApplicationDbContext> _contextMock;
    private readonly Mock<IPasswordHasher> _passwordHasherMock;
    private readonly Mock<IJwtTokenGenerator> _jwtGeneratorMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _contextMock = new Mock<IApplicationDbContext>();
        _passwordHasherMock = new Mock<IPasswordHasher>();
        _jwtGeneratorMock = new Mock<IJwtTokenGenerator>();
        
        _handler = new LoginCommandHandler(
            _passwordHasherMock.Object,
            _jwtGeneratorMock.Object,
            _contextMock.Object);
    }

    [Fact]
    public async Task Handle_ValidCredentials_ReturnsToken()
    {
        var user = new User
        {
            Id = 1,
            Username = "admin",
            Password = "hashed_password",
            RoleId = 1,
            Role = new Role { Id = 1, Code = "Moderator" }
        };

        var users = new List<User> { user }.AsQueryable().BuildMockDbSet();
        _contextMock.Setup(x => x.Users).Returns(users.Object);

        _passwordHasherMock
            .Setup(x => x.VerifyPassword("12345678", "hashed_password"))
            .Returns(true);

        _jwtGeneratorMock
            .Setup(x => x.GenerateToken(user))
            .Returns("fake_token");

        var command = new LoginCommand("admin", "12345678");

        var result = await _handler.Handle(command, CancellationToken.None);

        result.IsSuccess.Should().BeTrue();
        result.Value.Token.Should().Be("fake_token");
    }
}