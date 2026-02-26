namespace UserAPI.Tests.Unit.Repositories;

using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using UserAPI.Core.Domain.Entities;
using UserAPI.Core.Infrastructure.Data;
using UserAPI.Core.Infrastructure.Repositories;

/// <summary>
/// Unit tests for UserRepository
/// </summary>
[TestFixture]
public class UserRepositoryTests
{
    private ApplicationDbContext _context = null!;
    private UserRepository _userRepository = null!;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<ApplicationDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _context = new ApplicationDbContext(options);
        _userRepository = new UserRepository(_context);
    }

    [TearDown]
    public void TearDown()
    {
        _context?.Dispose();
    }

    [Test]
    public async Task CreateAsync_WithValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = new User("test@example.com", "John", "Doe", "hashed_password");

        // Act
        var result = await _userRepository.CreateAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test@example.com", result.Email);
        Assert.AreEqual("John", result.FirstName);
    }

    [Test]
    public async Task GetByIdAsync_WithExistingId_ReturnsUser()
    {
        // Arrange
        var user = new User("test@example.com", "John", "Doe", "hashed_password");
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByIdAsync(user.Id);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(user.Id, result.Id);
    }

    [Test]
    public async Task GetByIdAsync_WithNonExistingId_ReturnsNull()
    {
        // Arrange
        var nonExistingId = Guid.NewGuid();

        // Act
        var result = await _userRepository.GetByIdAsync(nonExistingId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User("test@example.com", "John", "Doe", "hashed_password");
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test@example.com", result.Email);
    }

    [Test]
    public async Task GetByEmailAsync_WithNonExistingEmail_ReturnsNull()
    {
        // Act
        var result = await _userRepository.GetByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllActiveUsers()
    {
        // Arrange
        var user1 = new User("user1@example.com", "John", "Doe", "hashed");
        var user2 = new User("user2@example.com", "Jane", "Smith", "hashed");
        await _userRepository.CreateAsync(user1);
        await _userRepository.CreateAsync(user2);

        // Act
        var result = await _userRepository.GetAllAsync();

        // Assert
        Assert.AreEqual(2, result.Count());
    }

    [Test]
    public async Task UpdateAsync_WithValidUser_UpdatesUser()
    {
        // Arrange
        var user = new User("test@example.com", "John", "Doe", "hashed_password");
        await _userRepository.CreateAsync(user);

        user.Update("UpdatedJohn", "UpdatedDoe");

        // Act
        var result = await _userRepository.UpdateAsync(user);

        // Assert
        Assert.AreEqual("UpdatedJohn", result.FirstName);
        Assert.AreEqual("UpdatedDoe", result.LastName);
    }

    [Test]
    public async Task DeleteAsync_WithValidId_DeactivatesUser()
    {
        // Arrange
        var user = new User("test@example.com", "John", "Doe", "hashed_password");
        await _userRepository.CreateAsync(user);

        // Act
        await _userRepository.DeleteAsync(user.Id);

        // Assert
        var result = await _userRepository.GetByIdAsync(user.Id);
        Assert.IsNull(result);
    }

    [Test]
    public async Task ExistsByEmailAsync_WithExistingEmail_ReturnsTrue()
    {
        // Arrange
        var user = new User("test@example.com", "John", "Doe", "hashed_password");
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.ExistsByEmailAsync("test@example.com");

        // Assert
        Assert.IsTrue(result);
    }

    [Test]
    public async Task ExistsByEmailAsync_WithNonExistingEmail_ReturnsFalse()
    {
        // Act
        var result = await _userRepository.ExistsByEmailAsync("nonexistent@example.com");

        // Assert
        Assert.IsFalse(result);
    }
}
