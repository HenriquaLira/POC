namespace UserAPI.Tests.Unit.Services;

using Moq;
using NUnit.Framework;
using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;
using UserAPI.Core.Application.Services;
using UserAPI.Core.Domain.Entities;

/// <summary>
/// Unit tests for UserService
/// </summary>
[TestFixture]
public class UserServiceTests
{
    private UserService _userService = null!;
    private Mock<IUserRepository> _mockUserRepository = null!;
    private Mock<IPasswordHasher> _mockPasswordHasher = null!;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();

        _userService = new UserService(_mockUserRepository.Object, _mockPasswordHasher.Object);
    }

    [Test]
    public async Task CreateAsync_WithValidDto_ReturnsUserResponseDto()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass123"
        };

        var hashedPassword = "hashed_password";
        var expectedUser = new User(
            createUserDto.Email,
            createUserDto.FirstName,
            createUserDto.LastName,
            hashedPassword
        );

        _mockPasswordHasher
            .Setup(x => x.Hash(createUserDto.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(createUserDto.Email))
            .ReturnsAsync(false);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedUser);

        // Act
        var result = await _userService.CreateAsync(createUserDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(createUserDto.Email, result.Email);
        Assert.AreEqual(createUserDto.FirstName, result.FirstName);
        Assert.AreEqual(createUserDto.LastName, result.LastName);

        _mockUserRepository.Verify(x => x.ExistsByEmailAsync(createUserDto.Email), Times.Once);
        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _mockPasswordHasher.Verify(x => x.Hash(createUserDto.Password), Times.Once);
    }

    [Test]
    public void CreateAsync_WithExistingEmail_ThrowsInvalidOperationException()
    {
        // Arrange
        var createUserDto = new CreateUserDto
        {
            Email = "existing@example.com",
            FirstName = "John",
            LastName = "Doe",
            Password = "SecurePass123"
        };

        _mockUserRepository
            .Setup(x => x.ExistsByEmailAsync(createUserDto.Email))
            .ReturnsAsync(true);

        // Act & Assert
        var ex = Assert.ThrowsAsync<InvalidOperationException>(
            () => _userService.CreateAsync(createUserDto)
        );

        Assert.IsTrue(ex.Message.Contains("already exists"));
    }

    [Test]
    public async Task GetByIdAsync_WithValidId_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var user = new User("test@example.com", "John", "Doe", "hashed")
        {
            Id = userId
        };

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(user);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(userId, result.Id);
        Assert.AreEqual(user.Email, result.Email);

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
    }

    [Test]
    public async Task GetByIdAsync_WithInvalidId_ReturnsNull()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _userService.GetByIdAsync(userId);

        // Assert
        Assert.IsNull(result);
    }

    [Test]
    public async Task GetAllAsync_ReturnsAllUsers()
    {
        // Arrange
        var users = new List<User>
        {
            new User("user1@example.com", "John", "Doe", "hashed") { Id = Guid.NewGuid() },
            new User("user2@example.com", "Jane", "Smith", "hashed") { Id = Guid.NewGuid() }
        };

        _mockUserRepository
            .Setup(x => x.GetAllAsync())
            .ReturnsAsync(users);

        // Act
        var result = await _userService.GetAllAsync();

        // Assert
        Assert.AreEqual(2, result.Count());

        _mockUserRepository.Verify(x => x.GetAllAsync(), Times.Once);
    }

    [Test]
    public async Task UpdateAsync_WithValidId_ReturnsUpdatedUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var updateUserDto = new UpdateUserDto
        {
            FirstName = "UpdatedJohn",
            LastName = "UpdatedDoe"
        };

        var existingUser = new User("test@example.com", "John", "Doe", "hashed")
        {
            Id = userId
        };

        _mockUserRepository
            .Setup(x => x.GetByIdAsync(userId))
            .ReturnsAsync(existingUser);

        _mockUserRepository
            .Setup(x => x.UpdateAsync(It.IsAny<User>()))
            .ReturnsAsync(existingUser);

        // Act
        var result = await _userService.UpdateAsync(userId, updateUserDto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(userId, result.Id);

        _mockUserRepository.Verify(x => x.GetByIdAsync(userId), Times.Once);
        _mockUserRepository.Verify(x => x.UpdateAsync(It.IsAny<User>()), Times.Once);
    }

    [Test]
    public async Task DeleteAsync_WithValidId_DeletesUser()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockUserRepository
            .Setup(x => x.DeleteAsync(userId))
            .Returns(Task.CompletedTask);

        // Act
        await _userService.DeleteAsync(userId);

        // Assert
        _mockUserRepository.Verify(x => x.DeleteAsync(userId), Times.Once);
    }
}
