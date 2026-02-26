namespace UserAPI.Tests.Unit.Services;

using NUnit.Framework;
using UserAPI.Core.Application.Interfaces;
using UserAPI.Core.Application.Services;

/// <summary>
/// Unit tests for JwtTokenService
/// </summary>
[TestFixture]
public class JwtTokenServiceTests
{
    private string _secretKey = null!;
    private IJwtTokenService _jwtTokenService = null!;

    [SetUp]
    public void Setup()
    {
        _secretKey = "ThisIsAVeryLongSecureSecretKeyThatMustBeAtLeast32CharactersLong!";
        _jwtTokenService = new JwtTokenService(_secretKey);
    }

    [Test]
    public void GenerateToken_WithValidInput_ReturnsToken()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = "test@example.com";

        // Act
        var token = _jwtTokenService.GenerateToken(userId, email);

        // Assert
        Assert.IsNotNull(token);
        Assert.IsNotEmpty(token);
        Assert.IsTrue(token.Contains("."));
    }

    [Test]
    public void GenerateToken_WithEmptyUserId_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.Empty;
        var email = "test@example.com";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _jwtTokenService.GenerateToken(userId, email));
    }

    [Test]
    public void GenerateToken_WithEmptyEmail_ThrowsArgumentException()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var email = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _jwtTokenService.GenerateToken(userId, email));
    }

    [Test]
    public void Constructor_WithShortSecretKey_ThrowsArgumentException()
    {
        // Arrange
        var shortKey = "short";

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new JwtTokenService(shortKey));
    }

    [Test]
    public void Constructor_WithEmptySecretKey_ThrowsArgumentException()
    {
        // Arrange
        var emptyKey = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => new JwtTokenService(emptyKey));
    }
}
