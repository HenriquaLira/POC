# User API - .NET 8 C# REST API

A complete, production-ready REST API application built with .NET 8 C#, following SOLID principles, with JWT authentication, Swagger documentation, and comprehensive unit tests.

## Features

✅ **SOLID Principles Implementation**
- Single Responsibility Principle (SRP)
- Open/Closed Principle (OCP)
- Liskov Substitution Principle (LSP)
- Interface Segregation Principle (ISP)
- Dependency Inversion Principle (DIP)

✅ **User Management**
- Create users with validation
- Read users by ID or list all
- Update user information
- Delete users (soft delete)
- Email uniqueness constraint

✅ **Authentication & Security**
- JWT token-based authentication
- BCrypt password hashing
- Secure password validation

✅ **API Documentation**
- Swagger/OpenAPI integration
- Detailed endpoint descriptions
- JWT authentication in Swagger UI

✅ **Testing**
- Unit tests with NUnit
- Mocking with Moq
- Repository pattern tests
- Service layer tests

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full installation)
- Visual Studio 2022 or VS Code

## Project Structure

```
UserAPI/
├── src/
│   ├── UserAPI.Core/
│   │   ├── Domain/
│   │   │   └── Entities/
│   │   ├── Application/
│   │   │   ├── DTOs/
│   │   │   ├── Services/
│   │   │   └── Interfaces/
│   │   └── Infrastructure/
│   │       ├── Data/
│   │       └── Repositories/
│   └── UserAPI.API/
│       ├── Controllers/
│       ├── Middlewares/
│       ├── Extensions/
│       ├── Program.cs
│       └── appsettings.json
└── tests/
    └── UserAPI.Tests.Unit/
        ├── Services/
        └── Repositories/
```

## Getting Started

### 1. Clone the Repository

```bash
git clone <repository-url>
cd UserAPI
```

### 2. Configure Database

Update the connection string in `src/UserAPI.API/appsettings.json`:

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=UserAPI;Trusted_Connection=true;"
}
```

### 3. Update JWT Secret

Change the JWT secret key in `appsettings.json` (must be at least 32 characters):

```json
"Jwt": {
  "SecretKey": "YourVeryLongSecureSecretKeyHere1234567890!",
  "Issuer": "UserAPI",
  "Audience": "UserAPIClient",
  "ExpirationMinutes": 60
}
```

### 4. Restore NuGet Packages

```bash
dotnet restore
```

### 5. Apply Database Migrations

```bash
cd src/UserAPI.API
dotnet ef database update
```

### 6. Run the Application

```bash
dotnet run
```

The API will start on `https://localhost:7245` (HTTPS) or `http://localhost:5245` (HTTP).

## API Endpoints

### Authentication

#### Login (Get JWT Token)
```http
POST /api/auth/login
Content-Type: application/json

{
  "email": "user@example.com",
  "password": "SecurePass123"
}
```

**Response:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "user": {
    "id": "guid",
    "email": "user@example.com",
    "firstName": "John",
    "lastName": "Doe",
    "createdAt": "2024-01-01T00:00:00Z",
    "isActive": true
  }
}
```

### Users

#### Create User
```http
POST /api/users
Content-Type: application/json

{
  "email": "newuser@example.com",
  "firstName": "Jane",
  "lastName": "Smith",
  "password": "SecurePass123"
}
```

**Response:** `201 Created`

#### Get User by ID
```http
GET /api/users/{id}
Authorization: Bearer <JWT_TOKEN>
```

**Response:** `200 OK`

#### Get All Users
```http
GET /api/users
Authorization: Bearer <JWT_TOKEN>
```

**Response:** `200 OK`

#### Update User
```http
PUT /api/users/{id}
Authorization: Bearer <JWT_TOKEN>
Content-Type: application/json

{
  "firstName": "UpdatedName",
  "lastName": "UpdatedLastName"
}
```

**Response:** `200 OK`

#### Delete User
```http
DELETE /api/users/{id}
Authorization: Bearer <JWT_TOKEN>
```

**Response:** `204 No Content`

## Testing

### Run Unit Tests

```bash
dotnet test
```

### Test Coverage

- **UserService**: CRUD operations, error handling
- **AuthenticationService**: Login, token generation
- **PasswordHasher**: Password hashing and verification
- **JwtTokenService**: Token generation and validation
- **UserRepository**: Database operations

## Validation Rules

### Password Requirements
- Minimum 8 characters
- At least one uppercase letter
- At least one lowercase letter
- At least one digit

### Email
- Valid email format
- Unique across the system

### Names
- Maximum 100 characters each
- Required fields

## Error Handling

The API includes comprehensive error handling:

```json
{
  "message": "Error description",
  "errors": {
    "fieldName": ["Error message 1", "Error message 2"]
  }
}
```

## Security

- ✅ JWT token-based authentication
- ✅ BCrypt password hashing
- ✅ SQL injection prevention via EF Core
- ✅ CORS configuration
- ✅ HTTPS enforcement
- ✅ Secure password validation

## Logging

The API uses structured logging with Microsoft.Extensions.Logging:

- Information level: User actions (login, create, update)
- Warning level: Failed attempts
- Error level: Exceptions and system errors

## Folder Structure Best Practices

```
Project Organization:
├── Domain Layer (Entities, Value Objects)
├── Application Layer (Services, DTOs, Validators)
├── Infrastructure Layer (Repositories, DbContext)
└── API Layer (Controllers, Middlewares, Extensions)
```

## Dependencies

- `Microsoft.EntityFrameworkCore` - ORM
- `Microsoft.AspNetCore.Authentication.JwtBearer` - JWT Auth
- `Swashbuckle.AspNetCore` - Swagger/OpenAPI
- `FluentValidation` - Input validation
- `BCrypt.Net-Next` - Password hashing
- `System.IdentityModel.Tokens.Jwt` - JWT token handling

## Contributing

When contributing to this project:

1. Follow SOLID principles
2. Write unit tests for new features
3. Add XML documentation comments
4. Update README with API changes
5. Use consistent naming conventions

## License

This project is provided as-is for educational and POC purposes.

## Support

For issues or questions, please refer to the inline documentation and code comments throughout the project.

---

**Created:** February 25, 2026  
**Framework:** .NET 8  
**Language:** C#  
**Pattern:** Clean Architecture with SOLID Principles
