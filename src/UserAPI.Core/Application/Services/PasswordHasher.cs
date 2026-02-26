namespace UserAPI.Core.Application.Services;

using UserAPI.Core.Application.Interfaces;

/// <summary>
/// Password hashing service using BCrypt algorithm
/// </summary>
public class PasswordHasher : IPasswordHasher
{
    /// <summary>
    /// Hash a password using BCrypt
    /// </summary>
    public string Hash(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            throw new ArgumentException("Password cannot be empty", nameof(password));

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Verify a password against its hash
    /// </summary>
    public bool Verify(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password) || string.IsNullOrWhiteSpace(hash))
            return false;

        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
