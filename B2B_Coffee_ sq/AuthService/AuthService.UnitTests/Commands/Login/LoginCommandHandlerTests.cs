using AuthService.Application.Commands.Login;
using AuthService.Application.Interfaces;
using AuthService.Domain.Entities;
using AuthService.Domain.Enums;
using FluentAssertions;
using MockQueryable.Moq;
using Moq;
using Xunit;

namespace AuthService.UnitTests.Commands.Login;

public class LoginCommandHandlerTests
{
    private readonly Mock<IAuthDbContext> _dbMock;
    private readonly Mock<IJwtService> _jwtMock;
    private readonly LoginCommandHandler _handler;

    public LoginCommandHandlerTests()
    {
        _dbMock = new Mock<IAuthDbContext>();
        _jwtMock = new Mock<IJwtService>();
        _handler = new LoginCommandHandler(_dbMock.Object, _jwtMock.Object);
    }

    [Fact]
    public async Task Handle_WithValidCredentials_ShouldReturnSuccess()
    {
        // Arrange
        var password = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new ApplicationUser
        {
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Status = UserStatus.Approved,
            Role = UserRole.Admin
        };

        var users = new List<ApplicationUser> { user }.BuildMockDbSet();
        _dbMock.Setup(d => d.Users).Returns(users.Object);
        _jwtMock.Setup(j => j.GenerateToken(It.IsAny<ApplicationUser>())).Returns("test-token");

        var command = new LoginCommand("test@example.com", password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeTrue();
        result.Token.Should().Be("test-token");
        result.Message.Should().Be("Login successful.");
    }

    [Fact]
    public async Task Handle_WithInvalidPassword_ShouldReturnFailure()
    {
        // Arrange
        var passwordHash = BCrypt.Net.BCrypt.HashPassword("CorrectPassword");
        var user = new ApplicationUser
        {
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Status = UserStatus.Approved
        };

        var users = new List<ApplicationUser> { user }.BuildMockDbSet();
        _dbMock.Setup(d => d.Users).Returns(users.Object);

        var command = new LoginCommand("test@example.com", "WrongPassword");

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Invalid email or password.");
    }

    [Fact]
    public async Task Handle_WhenUserIsRejected_ShouldReturnAccountRejectedMessage()
    {
        // Arrange
        var password = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new ApplicationUser
        {
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Status = UserStatus.Rejected
        };

        var users = new List<ApplicationUser> { user }.BuildMockDbSet();
        _dbMock.Setup(d => d.Users).Returns(users.Object);

        var command = new LoginCommand("test@example.com", password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Your account has been terminated or rejected. Please contact support.");
    }

    [Fact]
    public async Task Handle_WhenClientIsPendingApproval_ShouldReturnPendingApprovalMessage()
    {
        // Arrange
        var password = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new ApplicationUser
        {
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Status = UserStatus.Pending,
            Role = UserRole.Client
        };

        var users = new List<ApplicationUser> { user }.BuildMockDbSet();
        _dbMock.Setup(d => d.Users).Returns(users.Object);

        var command = new LoginCommand("test@example.com", password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Your account is pending Admin approval.");
    }

    [Fact]
    public async Task Handle_WhenClientIsPendingOtp_ShouldReturnVerifyOtpMessage()
    {
        // Arrange
        var password = "Password123!";
        var passwordHash = BCrypt.Net.BCrypt.HashPassword(password);
        var user = new ApplicationUser
        {
            Email = "test@example.com",
            PasswordHash = passwordHash,
            Status = UserStatus.PendingOtp,
            Role = UserRole.Client
        };

        var users = new List<ApplicationUser> { user }.BuildMockDbSet();
        _dbMock.Setup(d => d.Users).Returns(users.Object);

        var command = new LoginCommand("test@example.com", password);

        // Act
        var result = await _handler.Handle(command, CancellationToken.None);

        // Assert
        result.Success.Should().BeFalse();
        result.Message.Should().Be("Please verify your OTP first.");
    }
}
