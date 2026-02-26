namespace UserAPI.Tests.Unit.Services;

using Moq;
using NUnit.Framework;
using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;
using UserAPI.Core.Application.Services;
using UserAPI.Core.Domain.Entities;

/// <summary>
/// Unit tests for AuthenticationService
/// </summary>
[TestFixture]
public class AuthenticationServiceTests
{
    private AuthenticationService _authenticationService = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IPasswordHasher> _mockPasswordHasher = null!;
    private Mock<IJwtTokenService> _mockJwtTokenService = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockJwtTokenService = new Mock<IJwtTokenService>();

        _authenticationService = new AuthenticationService(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockJwtTokenService.Object
        );
    }

    [Test]
    public async Task AuthenticateAsync_WithValidCredentials_ReturnsLoginResponseDto()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "SecurePass123"
        };

        var user = new User(loginDto.Email, "John", "Doe", "hashed_password")
        {
            Id = Guid.NewGuid()
        };

        var token = "jwt_token_here";

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.Verify(loginDto.Password, user.PasswordHash))
            .Returns(true);

        _mockJwtTokenService
            .Setup(x => x.GenerateToken(user.Id, user.Email))
            .Returns(token);

        // Act
        var result = await _authenticationService.AuthenticateAsync(loginDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(token, result.Token);
        Assert.AreEqual(user.Email, result.User.Email);

        _mockUserRepository.Verify(x => x.GetByEmailAsync(loginDto.Email), Times.Once);
        _mockPasswordHasher.Verify(x => x.Verify(loginDto.Password, user.PasswordHash), Times.Once);
        _mockJwtTokenService.Verify(x => x.GenerateToken(user.Id, user.Email), Times.Once);
    }

    [Test]
    public void AuthenticateAsync_WithInvalidPassword_ThrowsInvalidOperationException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "test@example.com",
            Password = "WrongPassword"
        };

        var user = new User(loginDto.Email, "John", "Doe", "hashed_password")
        {
            Id = Guid.NewGuid()
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync(user);

        _mockPasswordHasher
            .Setup(x => x.Verify(loginDto.Password, user.PasswordHash))
            .Returns(false);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            () => _authenticationService.AuthenticateAsync(loginDto)
        );

        Assert.IsTrue(ex.Message.Contains("Invalid email or password"));
    }

    [Test]
    public void AuthenticateAsync_WithNonExistentEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var loginDto = new LoginDto
        {
            Email = "nonexistent@example.com",
            Password = "SecurePass123"
        };

        _mockUserRepository
            .Setup(x => x.GetByEmailAsync(loginDto.Email))
            .ReturnsAsync((User?)null);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            () => _authenticationService.AuthenticateAsync(loginDto)
        );

        Assert.IsTrue(ex.Message.Contains("Invalid email or password"));
    }
}
