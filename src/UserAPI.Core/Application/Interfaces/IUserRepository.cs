namespace UserAPI.Core.Application.Interfaces;

using UserAPI.Core.Domain.Entities;

/// <summary>
/// Interface for user repository operations
/// </summary>
public interface IUserRepository
{
    /// <summary>
    /// Create a new user
    /// </summary>
    Task<User> CreateAsync(User user);

    /// <summary>
    /// Get user by ID
    /// </summary>
    Task<User?> GetByIdAsync(Guid id);

    /// <summary>
    /// Get user by email
    /// </summary>
    Task<User?> GetByEmailAsync(string email);

    /// <summary>
    /// Get all users
    /// </summary>
    Task<IEnumerable<User>> GetAllAsync();

    /// <summary>
    /// Update an existing user
    /// </summary>
    Task<User> UpdateAsync(User user);

    /// <summary>
    /// Delete a user
    /// </summary>
    Task DeleteAsync(Guid id);

    /// <summary>
    /// Check if user exists by email
    /// </summary>
    Task<bool> ExistsByEmailAsync(string email);
}
