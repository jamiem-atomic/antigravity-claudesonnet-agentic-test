using Microsoft.Extensions.Configuration;
using Moq;
using VehicleMarketplace.Api.DTOs;
using VehicleMarketplace.Api.Services;

namespace VehicleMarketplace.Tests;

public class AuthServiceTests : TestBase
{
    private readonly AuthService _authService;
    private readonly Mock<IConfiguration> _mockConfig;

    public AuthServiceTests()
    {
        _mockConfig = new Mock<IConfiguration>();
        _mockConfig.Setup(c => c["Jwt:Key"]).Returns("MySuperSecretKeyForTestingPurposeOnly123!");
        _mockConfig.Setup(c => c["Jwt:Issuer"]).Returns("TestIssuer");
        _mockConfig.Setup(c => c["Jwt:Audience"]).Returns("TestAudience");

        _authService = new AuthService(_context, _mockConfig.Object);
    }

    [Fact]
    public async Task RegisterAsync_ShouldCreateUser_WhenValidRequest()
    {
        // Arrange
        var request = new RegisterRequest
        {
            Email = "newuser@example.com",
            Password = "Password123!",
            DisplayName = "New User"
        };

        // Act
        var result = await _authService.RegisterAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal(request.Email, result.User.Email);
        Assert.Equal(request.DisplayName, result.User.DisplayName);

        var savedUser = await _context.Users.FindAsync(result.User.Id);
        Assert.NotNull(savedUser);
    }

    [Fact]
    public async Task RegisterAsync_ShouldFail_WhenEmailExists()
    {
        // Arrange
        await SeedUsersAsync();
        var request = new RegisterRequest
        {
            Email = "test@example.com", // Already exists
            Password = "Password123!",
            DisplayName = "Duplicate User"
        };

        // Act & Assert
        await Assert.ThrowsAsync<ArgumentException>(() => _authService.RegisterAsync(request));
    }

    [Fact]
    public async Task LoginAsync_ShouldReturnToken_WhenCredentialsValid()
    {
        // Arrange
        await SeedUsersAsync();
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "Password123!"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.NotNull(result);
        Assert.NotNull(result.Token);
        Assert.Equal("test@example.com", result.User.Email);
    }

    [Fact]
    public async Task LoginAsync_ShouldFail_WhenPasswordInvalid()
    {
        // Arrange
        await SeedUsersAsync();
        var request = new LoginRequest
        {
            Email = "test@example.com",
            Password = "WrongPassword!"
        };

        // Act
        var result = await _authService.LoginAsync(request);

        // Assert
        Assert.Null(result);
    }
}
