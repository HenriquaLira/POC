namespace UserAPI.Core.Application.Services;

using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;

/// <summary>
/// Authentication service implementing SRP and DIP
/// </summary>
public class AuthenticationService : IAuthenticationService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthenticationService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
        _jwtTokenService = jwtTokenService ?? throw new ArgumentNullException(nameof(jwtTokenService));
    }

    /// <summary>
    /// Authenticate user with email and password
    /// </summary>
    public async Task<LoginResponseDto> AuthenticateAsync(LoginDto loginDto)
    {
        if (loginDto == null)
            throw new ArgumentNullException(nameof(loginDto));

        var user = await _userRepository.GetByEmailAsync(loginDto.Email);
        if (user == null || !_passwordHasher.Verify(loginDto.Password, user.PasswordHash))
            throw new InvalidOperationException("Invalid email or password");

        var token = _jwtTokenService.GenerateToken(user.Id, user.Email);

        return new LoginResponseDto
        {
            Token = token,
            User = new UserResponseDto
            {
                Id = user.Id,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                CreatedAt = user.CreatedAt,
                UpdatedAt = user.UpdatedAt,
                IsActive = user.IsActive
            }
        };
    }
}
