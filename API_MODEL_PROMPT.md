# Model Prompt: .NET C# API Application with SOLID Principles & Unit Tests

## Project Structure

```
MyAPI/
├── src/
│   ├── MyAPI.Core/
│   │   ├── Domain/
│   │   │   ├── Entities/
│   │   │   ├── ValueObjects/
│   │   │   └── Interfaces/
│   │   ├── Application/
│   │   │   ├── Services/
│   │   │   ├── DTOs/
│   │   │   └── Interfaces/
│   │   └── Infrastructure/
│   │       ├── Data/
│   │       ├── Repositories/
│   │       └── ExternalServices/
│   └── MyAPI.API/
│       ├── Controllers/
│       ├── Middlewares/
│       ├── Extensions/
│       └── Program.cs
└── tests/
    ├── MyAPI.Tests.Unit/
    ├── MyAPI.Tests.Integration/
    └── MyAPI.Tests.E2E/
```

---

## 1. SOLID Principles Implementation

### 1.1 Single Responsibility Principle (SRP)

**Problem:** A class doing too many things

```csharp
// ❌ Bad: Multiple responsibilities
public class UserService
{
    public void CreateUser(string email, string password)
    {
        // Validate user
        // Hash password
        // Save to database
        // Send email notification
    }
}

// ✅ Good: Single responsibility
public interface IUserRepository
{
    Task<User> CreateAsync(User user);
}

public interface IPasswordHasher
{
    string Hash(string password);
}

public interface IEmailService
{
    Task SendWelcomeEmailAsync(string email);
}

public class CreateUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IEmailService _emailService;

    public CreateUserService(IUserRepository userRepository, IPasswordHasher passwordHasher, IEmailService emailService)
    {
        _userRepository = userRepository;
        _passwordHasher = passwordHasher;
        _emailService = emailService;
    }

    public async Task<User> ExecuteAsync(CreateUserDto dto)
    {
        var hashedPassword = _passwordHasher.Hash(dto.Password);
        var user = new User(dto.Email, hashedPassword);
        
        var createdUser = await _userRepository.CreateAsync(user);
        await _emailService.SendWelcomeEmailAsync(createdUser.Email);

        return createdUser;
    }
}
```

---

### 1.2 Open/Closed Principle (OCP)

**Problem:** Modifying existing code to add new functionality

```csharp
// ❌ Bad: Need to modify for each new discount type
public class OrderService
{
    public decimal CalculateDiscount(Order order, string discountType)
    {
        if (discountType == "percentage")
            return order.Total * 0.1m;
        else if (discountType == "fixed")
            return 50;
        // New discounts require modifying this method!
    }
}

// ✅ Good: Extended through interfaces
public interface IDiscountStrategy
{
    decimal Calculate(Order order);
}

public class PercentageDiscount : IDiscountStrategy
{
    private readonly decimal _percentage;

    public PercentageDiscount(decimal percentage) => _percentage = percentage;

    public decimal Calculate(Order order) => order.Total * _percentage;
}

public class FixedDiscount : IDiscountStrategy
{
    private readonly decimal _amount;

    public FixedDiscount(decimal amount) => _amount = amount;

    public decimal Calculate(Order order) => _amount;
}

public class OrderService
{
    public decimal CalculateDiscount(Order order, IDiscountStrategy discountStrategy)
    {
        return discountStrategy.Calculate(order);
    }
}
```

---

### 1.3 Liskov Substitution Principle (LSP)

**Problem:** Derived classes break the base contract

```csharp
// ❌ Bad: Square violates rectangle behavior
public class Rectangle
{
    public virtual void SetWidth(decimal width) => Width = width;
    public virtual void SetHeight(decimal height) => Height = height;
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public decimal Area() => Width * Height;
}

public class Square : Rectangle
{
    public override void SetWidth(decimal width)
    {
        Width = width;
        Height = width; // Violates LSP!
    }

    public override void SetHeight(decimal height)
    {
        Width = height;
        Height = height;
    }
}

// ✅ Good: Proper abstraction
public abstract class Shape
{
    public abstract decimal Area();
}

public class Rectangle : Shape
{
    public decimal Width { get; set; }
    public decimal Height { get; set; }
    public override decimal Area() => Width * Height;
}

public class Square : Shape
{
    public decimal Side { get; set; }
    public override decimal Area() => Side * Side;
}
```

---

### 1.4 Interface Segregation Principle (ISP)

**Problem:** Classes forced to implement unused methods

```csharp
// ❌ Bad: Fat interface
public interface IWorker
{
    void Work();
    void Eat();
}

public class Robot : IWorker
{
    public void Work() => Console.WriteLine("Working");
    public void Eat() => throw new NotImplementedException(); // Forced!
}

// ✅ Good: Segregated interfaces
public interface IWorker
{
    void Work();
}

public interface IEater
{
    void Eat();
}

public class Robot : IWorker
{
    public void Work() => Console.WriteLine("Working");
}

public class Human : IWorker, IEater
{
    public void Work() => Console.WriteLine("Working");
    public void Eat() => Console.WriteLine("Eating");
}
```

---

### 1.5 Dependency Inversion Principle (DIP)

**Problem:** High-level modules depend on low-level modules

```csharp
// ❌ Bad: Direct dependency on concrete classes
public class UserService
{
    private readonly SqlServerDatabase _database = new();
    private readonly SmtpEmailService _emailService = new();

    public async Task CreateUserAsync(CreateUserDto dto)
    {
        var user = new User(dto.Email);
        _database.Save(user);
        _emailService.Send(user.Email, "Welcome!");
    }
}

// ✅ Good: Depends on abstractions
public interface IEmailService
{
    Task SendAsync(string to, string message);
}

public interface IUserRepository
{
    Task SaveAsync(User user);
}

public class CreateUserService
{
    private readonly IUserRepository _userRepository;
    private readonly IEmailService _emailService;

    public CreateUserService(IUserRepository userRepository, IEmailService emailService)
    {
        _userRepository = userRepository;
        _emailService = emailService;
    }

    public async Task ExecuteAsync(CreateUserDto dto)
    {
        var user = new User(dto.Email);
        await _userRepository.SaveAsync(user);
        await _emailService.SendAsync(user.Email, "Welcome!");
    }
}
```

---

## 2. Dependency Injection Setup

```csharp
// Program.cs
var builder = WebApplicationBuilder.CreateBuilder(args);

// Register repositories
builder.Services.AddScoped<IUserRepository, UserRepository>();
builder.Services.AddScoped<IProductRepository, ProductRepository>();

// Register services
builder.Services.AddScoped<CreateUserService>();
builder.Services.AddScoped<IPasswordHasher, BcryptPasswordHasher>();
builder.Services.AddScoped<IEmailService, SmtpEmailService>();

// Register other services
builder.Services.AddControllers();
builder.Services.AddSwaggerGen();

var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();
app.UseHttpsRedirection();
app.MapControllers();
app.Run();
```

---

## 3. Example Domain Model

```csharp
// User.cs (Domain Entity)
public class User
{
    public Guid Id { get; set; }
    public string Email { get; set; }
    public string PasswordHash { get; set; }
    public DateTime CreatedAt { get; set; }
    public bool IsActive { get; set; }

    public User(string email, string passwordHash)
    {
        Id = Guid.NewGuid();
        Email = email;
        PasswordHash = passwordHash;
        CreatedAt = DateTime.UtcNow;
        IsActive = true;
    }
}

// CreateUserDto.cs
public class CreateUserDto
{
    public string Email { get; set; }
    public string Password { get; set; }
}
```

---

## 4. Unit Testing Examples

### 4.1 Repository Tests

```csharp
// UserRepositoryTests.cs
[TestFixture]
public class UserRepositoryTests
{
    private IUserRepository _userRepository;
    private DbContext _dbContext;

    [SetUp]
    public void Setup()
    {
        var options = new DbContextOptionsBuilder<DbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        _dbContext = new DbContext(options);
        _userRepository = new UserRepository(_dbContext);
    }

    [Test]
    public async Task CreateAsync_WithValidUser_ReturnsCreatedUser()
    {
        // Arrange
        var user = new User("test@example.com", "hashedPassword");

        // Act
        var result = await _userRepository.CreateAsync(user);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test@example.com", result.Email);
    }

    [Test]
    public async Task GetByEmailAsync_WithExistingEmail_ReturnsUser()
    {
        // Arrange
        var user = new User("test@example.com", "hashedPassword");
        await _userRepository.CreateAsync(user);

        // Act
        var result = await _userRepository.GetByEmailAsync("test@example.com");

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual("test@example.com", result.Email);
    }

    [TearDown]
    public void TearDown()
    {
        _dbContext?.Dispose();
    }
}
```

### 4.2 Service Tests

```csharp
// CreateUserServiceTests.cs
[TestFixture]
public class CreateUserServiceTests
{
    private CreateUserService _createUserService;
    private Mock<IUserRepository> _mockUserRepository;
    private Mock<IPasswordHasher> _mockPasswordHasher;
    private Mock<IEmailService> _mockEmailService;

    [SetUp]
    public void Setup()
    {
        _mockUserRepository = new Mock<IUserRepository>();
        _mockPasswordHasher = new Mock<IPasswordHasher>();
        _mockEmailService = new Mock<IEmailService>();

        _createUserService = new CreateUserService(
            _mockUserRepository.Object,
            _mockPasswordHasher.Object,
            _mockEmailService.Object
        );
    }

    [Test]
    public async Task ExecuteAsync_WithValidDto_CreatesUserAndSendsEmail()
    {
        // Arrange
        var dto = new CreateUserDto { Email = "test@example.com", Password = "Password123!" };
        var hashedPassword = "hashed_password";
        var expectedUser = new User(dto.Email, hashedPassword);

        _mockPasswordHasher
            .Setup(x => x.Hash(dto.Password))
            .Returns(hashedPassword);

        _mockUserRepository
            .Setup(x => x.CreateAsync(It.IsAny<User>()))
            .ReturnsAsync(expectedUser);

        _mockEmailService
            .Setup(x => x.SendWelcomeEmailAsync(dto.Email))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _createUserService.ExecuteAsync(dto);

        // Assert
        Assert.IsNotNull(result);
        Assert.AreEqual(dto.Email, result.Email);

        _mockUserRepository.Verify(x => x.CreateAsync(It.IsAny<User>()), Times.Once);
        _mockEmailService.Verify(x => x.SendWelcomeEmailAsync(dto.Email), Times.Once);
    }

    [Test]
    public async Task ExecuteAsync_WithInvalidPassword_ThrowsException()
    {
        // Arrange
        var dto = new CreateUserDto { Email = "test@example.com", Password = "weak" };

        _mockPasswordHasher
            .Setup(x => x.Hash(It.IsAny<string>()))
            .Throws<ArgumentException>();

        // Act & Assert
        Assert.ThrowsAsync<ArgumentException>(() => _createUserService.ExecuteAsync(dto));
    }

    [TearDown]
    public void TearDown()
    {
        _mockUserRepository = null;
        _mockPasswordHasher = null;
        _mockEmailService = null;
    }
}
```

### 4.3 Controller Tests

```csharp
// UsersControllerTests.cs
[TestFixture]
public class UsersControllerTests
{
    private UsersController _controller;
    private Mock<CreateUserService> _mockCreateUserService;

    [SetUp]
    public void Setup()
    {
        _mockCreateUserService = new Mock<CreateUserService>();
        _controller = new UsersController(_mockCreateUserService.Object);
    }

    [Test]
    public async Task Create_WithValidDto_ReturnsOkResult()
    {
        // Arrange
        var dto = new CreateUserDto { Email = "test@example.com", Password = "Password123!" };
        var user = new User(dto.Email, "hashed");

        _mockCreateUserService
            .Setup(x => x.ExecuteAsync(dto))
            .ReturnsAsync(user);

        // Act
        var result = await _controller.Create(dto);

        // Assert
        Assert.IsInstanceOf<OkObjectResult>(result);
        var okResult = (OkObjectResult)result;
        Assert.AreEqual(200, okResult.StatusCode);
    }

    [Test]
    public async Task Create_WithDuplicateEmail_ReturnsBadRequest()
    {
        // Arrange
        var dto = new CreateUserDto { Email = "test@example.com", Password = "Password123!" };

        _mockCreateUserService
            .Setup(x => x.ExecuteAsync(dto))
            .ThrowsAsync(new InvalidOperationException("Email already exists"));

        // Act & Assert
        Assert.ThrowsAsync<InvalidOperationException>(() => _controller.Create(dto));
    }
}
```

---

## 5. Best Practices

### 5.1 Validation

```csharp
public class CreateUserValidator : AbstractValidator<CreateUserDto>
{
    public CreateUserValidator()
    {
        RuleFor(x => x.Email)
            .NotEmpty().WithMessage("Email is required")
            .EmailAddress().WithMessage("Email must be valid");

        RuleFor(x => x.Password)
            .NotEmpty().WithMessage("Password is required")
            .MinimumLength(8).WithMessage("Password must be at least 8 characters")
            .Matches(@"[A-Z]").WithMessage("Password must contain uppercase letters")
            .Matches(@"[0-9]").WithMessage("Password must contain numbers");
    }
}
```

### 5.2 Error Handling

```csharp
public class ErrorHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ErrorHandlingMiddleware> _logger;

    public ErrorHandlingMiddleware(RequestDelegate next, ILogger<ErrorHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred");
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        context.Response.ContentType = "application/json";
        context.Response.StatusCode = StatusCodes.Status500InternalServerError;

        return context.Response.WriteAsJsonAsync(new { message = "An error occurred" });
    }
}
```

### 5.3 Logging

```csharp
public class CreateUserService
{
    private readonly ILogger<CreateUserService> _logger;

    public async Task<User> ExecuteAsync(CreateUserDto dto)
    {
        _logger.LogInformation("Creating user with email: {Email}", dto.Email);

        try
        {
            var user = new User(dto.Email, "hashed");
            _logger.LogInformation("User created successfully: {UserId}", user.Id);
            return user;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create user: {Email}", dto.Email);
            throw;
        }
    }
}
```

---

## 6. Testing Framework Setup

```xml
<!-- .csproj -->
<ItemGroup>
    <PackageReference Include="NUnit" Version="3.13.3" />
    <PackageReference Include="NUnit3TestAdapter" Version="4.4.2" />
    <PackageReference Include="Moq" Version="4.16.1" />
    <PackageReference Include="FluentAssertions" Version="6.11.0" />
    <PackageReference Include="FluentValidation" Version="11.5.1" />
    <PackageReference Include="Microsoft.EntityFrameworkCore.InMemory" Version="7.0.0" />
</ItemGroup>
```

---

## Summary

This model provides:
- ✅ Clear separation of concerns (SRP)
- ✅ Extensibility without modification (OCP)
- ✅ Correct inheritance behavior (LSP)
- ✅ Focused interfaces (ISP)
- ✅ Abstraction-based design (DIP)
- ✅ Comprehensive unit tests with mocking
- ✅ Proper dependency injection
- ✅ Error handling and logging
- ✅ Input validation

Use this as a foundation for building scalable, maintainable .NET C# APIs!
