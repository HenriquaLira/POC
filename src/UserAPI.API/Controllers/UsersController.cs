namespace UserAPI.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;

/// <summary>
/// Users controller for managing user operations with JWT authentication
/// </summary>
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    private readonly ILogger<UsersController> _logger;

    public UsersController(IUserService userService, ILogger<UsersController> logger)
    {
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    [HttpPost]
    [AllowAnonymous]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<UserResponseDto>> Create([FromBody] CreateUserDto createUserDto)
    {
        try
        {
            _logger.LogInformation("Creating user with email: {Email}", createUserDto.Email);

            var user = await _userService.CreateAsync(createUserDto);

            _logger.LogInformation("User created successfully: {UserId}", user.Id);

            return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
        }
        catch (InvalidOperationException ex)
        {
            _logger.LogWarning("Failed to create user: {Message}", ex.Message);
            return Conflict(new { message = ex.Message });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating user");
            return BadRequest(new { message = "An error occurred while creating the user" });
        }
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
    {
        try
        {
            _logger.LogInformation("Getting user by ID: {UserId}", id);

            var user = await _userService.GetByIdAsync(id);

            if (user == null)
            {
                _logger.LogWarning("User not found: {UserId}", id);
                return NotFound(new { message = "User not found" });
            }

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving user");
            return BadRequest(new { message = "An error occurred while retrieving the user" });
        }
    }

    /// <summary>
    /// Get all users
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        try
        {
            _logger.LogInformation("Getting all users");

            var users = await _userService.GetAllAsync();

            return Ok(users);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error retrieving users");
            return BadRequest(new { message = "An error occurred while retrieving users" });
        }
    }

    /// <summary>
    /// Update user
    /// </summary>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<UserResponseDto>> Update(Guid id, [FromBody] UpdateUserDto updateUserDto)
    {
        try
        {
            _logger.LogInformation("Updating user: {UserId}", id);

            var user = await _userService.UpdateAsync(id, updateUserDto);

            if (user == null)
            {
                _logger.LogWarning("User not found for update: {UserId}", id);
                return NotFound(new { message = "User not found" });
            }

            _logger.LogInformation("User updated successfully: {UserId}", id);

            return Ok(user);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error updating user");
            return BadRequest(new { message = "An error occurred while updating the user" });
        }
    }

    /// <summary>
    /// Delete user
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult> Delete(Guid id)
    {
        try
        {
            _logger.LogInformation("Deleting user: {UserId}", id);

            await _userService.DeleteAsync(id);

            _logger.LogInformation("User deleted successfully: {UserId}", id);

            return NoContent();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting user");
            return BadRequest(new { message = "An error occurred while deleting the user" });
        }
    }
}
