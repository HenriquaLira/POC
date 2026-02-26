namespace UserAPI.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;

/// <summary>
/// Users controller for managing user operations with JWT authentication.
/// Exception handling is delegated to ExceptionHandlingMiddleware.
/// </summary>
[Route("api/v1/[controller]")]
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
        _logger.LogInformation("Creating user with email: {Email}", createUserDto.Email);

        var user = await _userService.CreateAsync(createUserDto);

        _logger.LogInformation("User created successfully: {UserId}", user.Id);

        return CreatedAtAction(nameof(GetById), new { id = user.Id }, user);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<UserResponseDto>> GetById(Guid id)
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

    /// <summary>
    /// Get all users (non-paginated, kept for backward compatibility)
    /// </summary>
    [HttpGet("all")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<IEnumerable<UserResponseDto>>> GetAll()
    {
        _logger.LogInformation("Getting all users");

        var users = await _userService.GetAllAsync();

        return Ok(users);
    }

    /// <summary>
    /// Get users with pagination
    /// </summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<ActionResult<PaginatedResponseDto<UserResponseDto>>> GetPaginated([FromQuery] PaginationQueryDto paginationQuery)
    {
        _logger.LogInformation("Getting users page {Page} with size {PageSize}", paginationQuery.Page, paginationQuery.PageSize);

        var result = await _userService.GetPaginatedAsync(paginationQuery);

        return Ok(result);
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

    /// <summary>
    /// Delete user (soft delete)
    /// </summary>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> Delete(Guid id)
    {
        _logger.LogInformation("Deleting user: {UserId}", id);

        await _userService.DeleteAsync(id);

        _logger.LogInformation("User deleted successfully: {UserId}", id);

        return NoContent();
    }
}
