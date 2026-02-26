namespace UserAPI.API.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;

/// <summary>
/// Authentication controller for user login and token generation.
/// Exception handling is delegated to ExceptionHandlingMiddleware.
/// </summary>
[Route("api/v1/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly IAuthenticationService _authenticationService;
    private readonly ILogger<AuthController> _logger;

    public AuthController(IAuthenticationService authenticationService, ILogger<AuthController> logger)
    {
        _authenticationService = authenticationService ?? throw new ArgumentNullException(nameof(authenticationService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Authenticate user and get JWT token
    /// </summary>
    [HttpPost("login")]
    [EnableRateLimiting("auth")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status429TooManyRequests)]
    public async Task<ActionResult<LoginResponseDto>> Login([FromBody] LoginDto loginDto)
    {
        _logger.LogInformation("Login attempt for email: {Email}", loginDto.Email);

        var response = await _authenticationService.AuthenticateAsync(loginDto);

        _logger.LogInformation("Successful login for user: {UserId}", response.User.Id);

        return Ok(response);
    }
}
