namespace UserAPI.Tests.Unit.Services;

using NUnit.Framework;
using UserAPI.Core.Application.Interfaces;
using UserAPI.Core.Application.Services;

/// <summary>
/// Unit tests for PasswordHasher
/// </summary>
[TestFixture]
public class PasswordHasherTests
{
    private IPasswordHasher _passwordHasher = null!;

    [SetUp]
    public void Setup()
    {
        _passwordHasher = new PasswordHasher();
    }

    [Test]
    public void Hash_WithValidPassword_ReturnsHash()
    {
        // Arrange
        var password = "SecurePass123";

        // Act
        var hash = _passwordHasher.Hash(password);

        // Assert
        Assert.IsNotNull(hash);
        Assert.IsNotEmpty(hash);
        Assert.AreNotEqual(password, hash);
    }

    [Test]
    public void Verify_WithCorrectPassword_ReturnsTrue()
    {
        // Arrange
        var password = "SecurePass123";
        var hash = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(password, hash);

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public void Verify_WithIncorrectPassword_ReturnsFalse()
    {
        // Arrange
        var password = "SecurePass123";
        var incorrectPassword = "WrongPassword123";
        var hash = _passwordHasher.Hash(password);

        // Act
        var result = _passwordHasher.Verify(incorrectPassword, hash);

        // Assert
        Assert.IsFalse(result);
    }

    [Test]
    public void Hash_WithEmptyPassword_ThrowsArgumentException()
    {
        // Arrange
        var password = string.Empty;

        // Act & Assert
        Assert.Throws<ArgumentException>(() => _passwordHasher.Hash(password));
    }
}
