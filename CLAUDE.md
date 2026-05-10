# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Run (development with hot reload)
dotnet watch run

# Build
dotnet build

# Database migrations
dotnet ef migrations add <MigrationName>   # create a new migration
dotnet ef database update                  # apply pending migrations
dotnet ef migrations remove                # remove the last (unapplied) migration
dotnet ef database update <MigrationName>  # roll back to a specific migration
dotnet ef migrations list                  # list all migrations and their status
dotnet ef database drop                    # drop the database
```

There are no automated tests configured in this project.

## Development URLs

When running locally (`dotnet watch run` / `dotnet run`):

- HTTPS: `https://localhost:5001`
- HTTP: `http://localhost:5000`
- Swagger UI: `https://localhost:5001/swagger`

## Architecture

ASP.NET Core 9.0 REST API using Vertical Slice Architecture with Minimal APIs.

```
HTTP Request → Endpoint → Handler (Validator → Business Logic → DataContext) → Result<T> → IResult
```

### Folder Layout

```
bike-club-api/
├── Features/                  # One folder per resource, one sub-folder per operation
│   ├── Account/
│   │   ├── Login/             # LoginRequest, LoginResponse, LoginHandler, LoginEndpoint, LoginValidator
│   │   └── Register/
│   ├── Address/
│   │   ├── GetAddress/
│   │   ├── CreateAddress/
│   │   └── ...
│   ├── Bike/
│   ├── Category/
│   ├── Difficulty/
│   ├── Gender/
│   ├── Purchase/
│   ├── Role/
│   ├── ShopCart/
│   │   └── Shared/            # Feature-local shared logic (e.g., TotalAmountCalculator)
│   ├── Tour/
│   └── User/
├── SharedKernel/
│   ├── IEndpoint.cs           # Marker interface: void MapEndpoint(IEndpointRouteBuilder app)
│   ├── Results/               # Result, Result<T>, Error, ErrorType, ValidationResult, ResultExtensions
│   ├── Http/                  # ResultExtensions.ToIResult() — maps Result to IResult (status + body)
│   ├── Services/              # ITokenService / TokenService, ICryptographerService / CryptographerService
│   ├── Static/                # RoleStatic, GenderStatic (constant string values)
│   └── Settings.cs            # Typed config: Secret, TokenExpirationHours
├── Domain/
│   └── Entities/              # EF Core entities — no validation annotations; mapping via Fluent API only
├── Extensions/                # One extension class per startup concern
│   ├── AuthenticationExtensions.cs
│   ├── CompressionExtensions.cs
│   ├── CorsExtensions.cs
│   ├── DataContextExtensions.cs
│   ├── EndpointExtensions.cs  # AddEndpoints (reflection scan) + MapEndpoints
│   ├── ExceptionHandlingExtensions.cs
│   ├── HandlerExtensions.cs   # AddHandlers (scans Features.* for *Handler types)
│   ├── JsonExtensions.cs      # ConfigureHttpJsonOptions (ReferenceHandler.IgnoreCycles)
│   ├── OutputCacheExtensions.cs
│   ├── ServicesExtensions.cs
│   ├── SettingsExtensions.cs
│   ├── StaticFilesExtensions.cs
│   ├── SwaggerExtensions.cs
│   └── ValidationExtensions.cs
├── Infrastructure/
│   └── ExceptionHandlers/     # UniqueConstraintExceptionHandler, ConcurrencyExceptionHandler, GlobalExceptionHandler
├── Data/
│   ├── Configurations/        # EF Fluent API mappings (IEntityTypeConfiguration<T>)
│   ├── Extensions/            # ModelBuilder.Seed() helper
│   ├── Seed/                  # Initial data for lookup tables and admin user
│   └── DataContext.cs         # Main DbContext (11 DbSet<T> properties)
├── Migrations/                # EF Core auto-generated migration files
├── Resources/Images/          # Static images served at /Resources
├── Properties/launchSettings.json
├── Program.cs                 # Composition root — extension method calls + middleware + app.Run()
├── appsettings.json
└── appsettings.Development.json
```

### Feature Slice Convention

Each operation under `Features/<Feature>/<Operation>/` contains:

- `[Operation]Request.cs` — `record` for the input payload (omit when no body/query input).
- `[Operation]Response.cs` — `record` for the returned data shape.
- `[Operation]Endpoint.cs` — Minimal API endpoint implementing `IEndpoint`.
- `[Operation]Handler.cs` — Orchestration class invoked by the endpoint.
- `[Operation]Validator.cs` — `AbstractValidator<TRequest>` (only when there is a request to validate).

### Result Pattern

Every handler returns `Result` or `Result<T>` (see the `result-pattern` skill). Handlers **never throw** for business failures — they return `Result.Failure(error)`.

`ResultExtensions.ToIResult()` in `SharedKernel/Http/` maps a `Result`/`Result<T>` to an `IResult`:

| Scenario | Status | Body |
|---|---|---|
| `Result<T>.Success` | 200 OK | Full `Result<T>` object (`value` holds the payload) |
| `Result.Success` (non-generic) | 204 No Content | None |
| `ErrorType.Validation` | 400 Bad Request | `Result` envelope with `errors[]` |
| `ErrorType.Failure` (default) | 400 Bad Request | `Result` envelope |
| `ErrorType.NotFound` | 404 Not Found | `Result` envelope |
| `ErrorType.Conflict` | 409 Conflict | `Result` envelope |
| `ErrorType.Unauthorized` | 401 Unauthorized | None (framework limitation) |
| `ErrorType.Forbidden` | 403 Forbidden | None (framework limitation) |

> Endpoints whose success path returns `204 NoContent` (non-generic `Result.Success`) still carry the `Result` envelope on **failure** paths — only the success path is body-less.

`ProblemDetails` is **not** used anywhere in this project.

### Response Envelope

Every endpoint that produces a response body serializes the `Result`/`Result<T>` envelope:

- **Success**: `{ isSuccess: true, isFailure: false, error: { code: "", description: "" }, value: <payload> }`
- **Failure**: `{ isSuccess: false, isFailure: true, error: { code: "...", description: "...", type: "..." }, value: null }`

Clients read the success payload from `response.value`; failures from `response.error`.

### FluentValidation

Each operation that takes input has a corresponding `[Operation]Validator : AbstractValidator<[Operation]Request>`. Validators are registered automatically via `AddValidatorsFromAssemblyContaining<Program>()` and resolved by handlers through constructor injection as `IValidator<TRequest>`. Handlers invoke validation before any business logic and return `Result.Failure` with a `ValidationResult` payload on failure.

Entity classes under `Domain/Entities/` carry **no** `System.ComponentModel.DataAnnotations` validation attributes — all validation lives in FluentValidation validators.

### Endpoint Auto-Registration

Any class implementing `IEndpoint` is automatically discovered via reflection (`AddEndpoints` scans the assembly) and mapped at startup (`MapEndpoints` invokes `MapEndpoint(app)` on each). **No edits to `Program.cs` are needed when adding a new feature** — create the operation folder with an `IEndpoint` implementation and run the app.

### Auth Flow

POST `/v1/accounts/login` or `/v1/accounts/register` → JWT returned → client sends `Authorization: Bearer <token>` → role-based access via `.RequireAuthorization(...)` on endpoints.

Two roles: `Monitor` (admin) and `Cyclist` (regular user), defined in `SharedKernel/Static/RoleStatic.cs`.

### Configuration

`SharedKernel/Settings.cs` holds `Secret` (JWT signing key) and `TokenExpirationHours`, populated from `appsettings.json` at startup via `app.LoadSettings()`.

### Static Files

Images live in `Resources/Images/` and are served at `/Resources`. The path is configured via `ResourcesImagesPath` in `appsettings.json`. The `Resources/` folder **must** stay at the project root (required by `.csproj` Content rules and static-files middleware).

## NuGet Packages

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.1 | JWT Bearer token authentication |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.1 | EF Core SQL Server database provider |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.1 | EF Core tooling support (migrations CLI) |
| `Microsoft.EntityFrameworkCore.InMemory` | 9.0.1 | In-memory database provider |
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger/OpenAPI documentation |
| `FluentValidation` | 12.1.1 | Request validation |
| `FluentValidation.DependencyInjectionExtensions` | 12.1.1 | Auto-register validators from assembly |
