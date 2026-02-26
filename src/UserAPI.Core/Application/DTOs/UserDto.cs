namespace UserAPI.Core.Application.DTOs;

/// <summary>
/// DTO for creating a new user
/// </summary>
public record CreateUserDto
{
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// DTO for updating user information
/// </summary>
public record UpdateUserDto
{
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
}

/// <summary>
/// DTO for user response
/// </summary>
public record UserResponseDto
{
    public Guid Id { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public DateTime CreatedAt { get; init; }
    public DateTime? UpdatedAt { get; init; }
    public bool IsActive { get; init; }
}

/// <summary>
/// DTO for authentication login
/// </summary>
public record LoginDto
{
    public string Email { get; init; } = string.Empty;
    public string Password { get; init; } = string.Empty;
}

/// <summary>
/// DTO for authentication response with JWT token
/// </summary>
public record LoginResponseDto
{
    public string Token { get; init; } = string.Empty;
    public UserResponseDto User { get; init; } = new();
}

/// <summary>
/// DTO for paginated responses
/// </summary>
public record PaginatedResponseDto<T>
{
    public IEnumerable<T> Items { get; init; } = Enumerable.Empty<T>();
    public int TotalCount { get; init; }
    public int Page { get; init; }
    public int PageSize { get; init; }
    public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
    public bool HasNextPage => Page < TotalPages;
    public bool HasPreviousPage => Page > 1;
}

/// <summary>
/// DTO for pagination query parameters
/// </summary>
public record PaginationQueryDto
{
    private const int MaxPageSize = 50;
    private const int DefaultPageSize = 10;

    private readonly int _page = 1;
    private readonly int _pageSize = DefaultPageSize;

    public int Page
    {
        get => _page;
        init => _page = value < 1 ? 1 : value;
    }

    public int PageSize
    {
        get => _pageSize;
        init => _pageSize = value > MaxPageSize ? MaxPageSize : value < 1 ? 1 : value;
    }
}
