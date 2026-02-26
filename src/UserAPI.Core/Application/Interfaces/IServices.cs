namespace UserAPI.Core.Application.Interfaces;

using UserAPI.Core.Application.DTOs;

/// <summary>
/// Interface for password hashing operations
/// </summary>
public interface IPasswordHasher
{
    /// <summary>
    /// Hash a password
    /// </summary>
    string Hash(string password);

    /// <summary>
    /// Verify a password against its hash
    /// </summary>
    bool Verify(string password, string hash);
}

/// <summary>
/// Interface for JWT token service
/// </summary>
public interface IJwtTokenService
{
    /// <summary>
    /// Generate a JWT token for a user
    /// </summary>
    string GenerateToken(Guid userId, string email);
}

/// <summary>
/// Interface for authentication service
/// </summary>
public interface IAuthenticationService
{
    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto);
}

/// <summary>
/// Interface for user service operations
/// </summary>
public interface IUserService
{
    /// <summary>
    /// Create a new user
    /// </summary>
    Task<UserResponseDto> CreateAsync(CreateUserDto createUserDto);

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<UserResponseDto?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get all users
    /// </summary>
    Task<IEnumerable<UserResponseDto>> GetAllAsync();

    /// <summary>
    /// Update user
    /// </summary>
    Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto);

    /// <summary>
    /// Delete user
    /// </summary>
    Task DeleteAsync(Guid id);
}
