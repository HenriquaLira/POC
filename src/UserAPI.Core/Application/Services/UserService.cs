namespace UserAPI.Core.Application.Services;

using UserAPI.Core.Application.DTOs;
using UserAPI.Core.Application.Interfaces;
using UserAPI.Core.Domain.Entities;
using UserAPI.Core.Domain.Exceptions;

/// <summary>
/// User service implementing SRP and DIP
/// </summary>
public class UserService : IUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;

    public UserService(
        IUserRepository userRepository,
        IPasswordHasher passwordHasher)
    {
        _userRepository = userRepository ?? throw new ArgumentNullException(nameof(userRepository));
        _passwordHasher = passwordHasher ?? throw new ArgumentNullException(nameof(passwordHasher));
    }

    /// <summary>
    /// Create a new user
    /// </summary>
    public async Task<UserResponseDto> CreateAsync(CreateUserDto createUserDto)
    {
        if (createUserDto == null)
            throw new ArgumentNullException(nameof(createUserDto));

        var exists = await _userRepository.ExistsByEmailAsync(createUserDto.Email);
        if (exists)
            throw new DuplicateException("User", "email", createUserDto.Email);

        var hashedPassword = _passwordHasher.Hash(createUserDto.Password);
        var user = new User(
            createUserDto.Email,
            createUserDto.FirstName,
            createUserDto.LastName,
            hashedPassword
        );

        var createdUser = await _userRepository.CreateAsync(user);

        return MapToResponseDto(createdUser);
    }

    /// <summary>
    /// Get user by ID
    /// </summary>
    public async Task<UserResponseDto?> GetByIdAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));

        var user = await _userRepository.GetByIdAsync(id);
        return user != null ? MapToResponseDto(user) : null;
    }

    /// <summary>
    /// Get all users
    /// </summary>
    public async Task<IEnumerable<UserResponseDto>> GetAllAsync()
    {
        var users = await _userRepository.GetAllAsync();
        return users.Select(MapToResponseDto);
    }

    /// <summary>
    /// Get users with pagination
    /// </summary>
    public async Task<PaginatedResponseDto<UserResponseDto>> GetPaginatedAsync(PaginationQueryDto paginationQuery)
    {
        if (paginationQuery == null)
            throw new ArgumentNullException(nameof(paginationQuery));

        var (items, totalCount) = await _userRepository.GetPaginatedAsync(paginationQuery.Page, paginationQuery.PageSize);

        return new PaginatedResponseDto<UserResponseDto>
        {
            Items = items.Select(MapToResponseDto),
            TotalCount = totalCount,
            Page = paginationQuery.Page,
            PageSize = paginationQuery.PageSize
        };
    }

    /// <summary>
    /// Update user
    /// </summary>
    public async Task<UserResponseDto?> UpdateAsync(Guid id, UpdateUserDto updateUserDto)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));

        if (updateUserDto == null)
            throw new ArgumentNullException(nameof(updateUserDto));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            return null;

        user.Update(updateUserDto.FirstName, updateUserDto.LastName);
        var updatedUser = await _userRepository.UpdateAsync(user);

        return MapToResponseDto(updatedUser);
    }

    /// <summary>
    /// Delete user
    /// </summary>
    public async Task DeleteAsync(Guid id)
    {
        if (id == Guid.Empty)
            throw new ArgumentException("User ID cannot be empty", nameof(id));

        var user = await _userRepository.GetByIdAsync(id);
        if (user == null)
            throw new NotFoundException("User", id);

        await _userRepository.DeleteAsync(id);
    }

    /// <summary>
    /// Map User entity to UserResponseDto
    /// </summary>
    private static UserResponseDto MapToResponseDto(User user)
    {
        return new UserResponseDto
        {
            Id = user.Id,
            Email = user.Email,
            FirstName = user.FirstName,
            LastName = user.LastName,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            IsActive = user.IsActive
        };
    }
}
