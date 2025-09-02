# CompanyApiProject

![.NET](https://img.shields.io/badge/.NET-8.0-blueviolet)
![ASP.NET Core](https://img.shields.io/badge/ASP.NET%20Core-Web%20API-green)
![License](https://img.shields.io/badge/License-MIT-blue)

A simple, clean, and extensible Web API for managing companies and related data. This project serves as a backend service implementing CRUD operations and basic business logic for a company domain. It's built with C# using ASP.NET Core, making it suitable as a starting point for production-ready APIs.

## Table of Contents

- [About](#about)
- [Features](#features)
- [Architecture & Patterns](#architecture--patterns)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Configuration](#configuration)
- [API Endpoints](#api-endpoints)
- [Authentication & Authorization](#authentication--authorization)
- [Testing](#testing)
- [Deployment](#deployment)
- [Contributing](#contributing)
- [License](#license)

## About

CompanyApiProject is a backend Web API designed for handling company-related data, such as creating, reading, updating, and deleting company records. It demonstrates best practices for building scalable RESTful services in .NET. The project is structured to be easily extensible, allowing you to add more features like employee management, invoicing, or integrations with external services.

This repository is ideal for:
- Developers learning ASP.NET Core Web APIs.
- Teams needing a boilerplate for company management systems.
- Projects requiring a clean architecture with separation of concerns.

Repository: [OmarAnalog/CompanyApiProject](https://github.com/OmarAnalog/CompanyApiProject)

## Features

- **RESTful Endpoints**: Full CRUD operations for company resources (e.g., GET/POST/PUT/DELETE for companies).
- **Layered Architecture**: Separates concerns into Controllers, Application/Services, Domain, and Infrastructure layers for maintainability.
- **Data Validation**: Built-in validation for requests using Data Annotations and optional FluentValidation.
- **Error Handling**: Global exception handling with meaningful error responses.
- **Logging**: Integrated logging with Serilog or ASP.NET Core's built-in logger.
- **Database Support**: ORM integration for persistent storage.
- **Extensibility**: Easy to add features like pagination, filtering, and sorting for queries.

## Architecture & Patterns

The project follows a clean, layered architecture inspired by Onion/Clean Architecture principles to ensure loose coupling and testability:

- **API Layer**: Contains controllers that handle HTTP requests and responses.
- **Application Layer**: Houses business logic, services, commands, and queries (using MediatR for CQRS if enabled).
- **Domain Layer**: Defines core entities, value objects, and domain events.
- **Infrastructure Layer**: Manages data access (e.g., repositories, unit of work) and external dependencies like databases.

Key Patterns:
- **Repository Pattern**: Abstracts data access to allow swapping databases without changing business logic.
- **Unit of Work**: Ensures atomic operations across multiple repositories.
- **CQRS (Optional)**: Separate command (write) and query (read) paths using MediatR for better scalability.
- **Dependency Injection**: All services are registered via ASP.NET Core's built-in DI container.

This structure makes the project modular— you can replace layers (e.g., switch from EF Core to Dapper) with minimal changes.

## Tech Stack

- **.NET 8.0**: The runtime and SDK for building the API.
- **ASP.NET Core Web API**: For creating RESTful services.
- **Entity Framework Core**: ORM for database interactions (code-first approach with migrations).
- **MediatR**: For implementing CQRS and mediating requests (optional but recommended for complex logic).
- **AutoMapper**: For mapping between entities and DTOs to avoid tight coupling.
- **FluentValidation**: For validating incoming requests and configurations.
- **Serilog**: Structured logging with console and file sinks.
- **Swagger/OpenAPI**: Integrated for API documentation and testing via Swashbuckle.
- **Database**: SQL Server (default), with support for PostgreSQL or SQLite via configuration changes.

## Prerequisites

Before setting up the project, ensure you have the following installed:

- **.NET SDK 8.0+**: Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download).
- **SQL Server**: LocalDB or a full instance (Express edition is fine for development). Alternatively, use PostgreSQL or SQLite.
- **Visual Studio 2022+** or **VS Code** with C# extensions: For editing and debugging.
- **Git**: For cloning the repository.
- (Optional) **Docker**: If you want to run the API in a container.
- (Optional) **Postman** or **Swagger UI**: For testing API endpoints.

## Getting Started

Follow these steps to get the project running locally:

1. **Clone the Repository**:
   ```
   git clone https://github.com/OmarAnalog/CompanyApiProject.git
   cd CompanyApiProject
   ```

2. **Restore Dependencies**:
   ```
   dotnet restore
   ```

3. **Set Up the Database**:
   - Update the connection string in `appsettings.json` or `appsettings.Development.json` to point to your database server.
   - Apply migrations to create the database schema:
     ```
     dotnet ef migrations add InitialCreate
     dotnet ef database update
     ```

4. **Build the Project**:
   ```
   dotnet build
   ```

5. **Run the API**:
   ```
   dotnet run
   ```
   - The API will start at `https://localhost:5001` (or the port specified in `launchSettings.json`).
   - Access Swagger UI at `https://localhost:5001/swagger` for interactive documentation.

6. (Optional) **Run with Docker**:
   - Build the Docker image:
     ```
     docker build -t companyapi .
     ```
   - Run the container:
     ```
     docker run -p 8080:80 companyapi
     ```

## Configuration

Configuration is handled via `appsettings.json` and environment variables for flexibility across environments (Development, Staging, Production).

- **Database Connection**: Set `ConnectionStrings:DefaultConnection` to your DB server.
- **Logging Levels**: Adjust Serilog settings for verbosity.
- **JWT Settings** (if auth is enabled): Add keys for token signing.
- Environment Variables: Override settings like `ASPNETCORE_ENVIRONMENT=Production` for production mode.

Use `IOptions` pattern for strongly-typed configuration access in code.

## API Endpoints

The API follows REST conventions. Base URL: `/api/companies`

- **GET /api/companies**: Retrieve all companies (supports pagination: `?page=1&size=10`).
- **GET /api/companies/{id}**: Get a company by ID.
- **POST /api/companies**: Create a new company (JSON body: `{ "name": "Example Corp", "address": "123 Street" }`).
- **PUT /api/companies/{id}**: Update an existing company.
- **DELETE /api/companies/{id}**: Delete a company.

Responses include standard HTTP status codes (200 OK, 201 Created, 400 Bad Request, etc.). Use Swagger for full details and testing.

## Authentication & Authorization

Currently, the API is unauthenticated for simplicity. To add security:

1. Install `Microsoft.AspNetCore.Authentication.JwtBearer`.
2. Configure JWT in `Program.cs`:
   ```csharp
   builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
       .AddJwtBearer(options => { /* configure */ });
   ```
3. Apply `[Authorize]` attributes to controllers/endpoints.
4. Generate tokens via a login endpoint (e.g., using Identity or custom auth).

Roles/Policies can be added for fine-grained access (e.g., Admin vs. User).

## Testing

The project includes placeholders for unit and integration tests using xUnit and Moq.

- **Run Tests**:
  ```
  dotnet test
  ```

- Add tests in the `Tests` project:
  - Unit tests for services and repositories.
  - Integration tests for controllers using `WebApplicationFactory`.

Aim for high coverage on business logic.

## Deployment

- **Azure/AWS/Heroku**: Deploy as a Web App or container.
- **Docker**: Use the provided `Dockerfile` for containerization.
- **CI/CD**: Set up GitHub Actions for automated builds/tests/deployments.
- Monitor with Application Insights or similar for production.

Ensure secrets (e.g., connection strings) are managed via environment variables or secret managers.

## Contributing

Contributions are welcome! Please follow these steps:

1. Fork the repository.
2. Create a feature branch: `git checkout -b feature/YourFeature`.
3. Commit changes: `git commit -m 'Add YourFeature'`.
4. Push to the branch: `git push origin feature/YourFeature`.
5. Open a Pull Request.

Follow code style guidelines (e.g., use EditorConfig) and add tests for new features.

## License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

---

If you encounter issues or have suggestions, open an issue on GitHub. Happy coding! 🚀
