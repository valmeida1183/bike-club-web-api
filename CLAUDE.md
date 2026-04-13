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

ASP.NET Core 9.0 REST API with a layered architecture:

```
Controllers → Services / Data Layer → Models → SQL Server (EF Core)
```

**Layers:**
- `Controllers/` — REST endpoints, all under the `v1/` route prefix. Handle HTTP, validate `ModelState`, delegate to EF Core directly or via services.
- `Services/` — Cross-cutting concerns: `TokenService` (JWT generation), `CryptographerService` (PBKDF2 hashing), `ExceptionHandlerService` (unified error responses).
- `Data/DataContext.cs` — Main `DbContext` with 11 `DbSet<T>` properties.
- `Data/Configurations/` — EF Fluent API mappings via `IEntityTypeConfiguration<T>`, one per entity.
- `Data/Seed/` — Initial data seeded via `ModelBuilder` extension in `Data/Extensions/`.
- `Models/` — Domain entities with data annotation validation.
- `Static/` — Constant classes for role and gender string values (`RoleStatic`, `GenderStatic`).

### Project Structure

```
bike-club-api/
├── Controllers/               # REST API endpoints (one file per resource)
│   ├── AccountController.cs
│   ├── AddressController.cs
│   ├── BikeController.cs
│   ├── CategoryController.cs
│   ├── DifficultyController.cs
│   ├── GenderController.cs
│   ├── PurchaseController.cs
│   ├── RoleController.cs
│   ├── ShopCartController.cs
│   ├── TourController.cs
│   └── UserController.cs
├── Data/
│   ├── Configurations/        # EF Fluent API mappings (IEntityTypeConfiguration<T>)
│   ├── Extensions/            # ModelBuilder.Seed() helper
│   ├── Seed/                  # Initial data for lookup tables and admin user
│   └── DataContext.cs         # Main DbContext
├── Migrations/                # EF Core auto-generated migration files
├── Models/                    # Domain entities with data annotation validation
│   ├── Address.cs
│   ├── Bike.cs
│   ├── Category.cs
│   ├── Difficulty.cs
│   ├── Gender.cs
│   ├── Purchase.cs
│   ├── Role.cs
│   ├── ShopCart.cs
│   ├── Tour.cs
│   ├── TourParticipant.cs
│   └── User.cs
├── Resources/Images/          # Static images served at /Resources
├── Services/                  # Cross-cutting services (token, crypto, exceptions)
├── Static/                    # Constant string values (RoleStatic, GenderStatic)
├── Properties/launchSettings.json
├── Program.cs                 # App startup, DI registration, middleware pipeline
├── Settings.cs                # Typed config (Secret, TokenExpirationHours)
├── appsettings.json
└── appsettings.Development.json
```

**Auth flow:** POST `/v1/accounts/login` or `/v1/accounts/register` → JWT returned → client sends `Authorization: Bearer <token>` → role-based access via `[Authorize(Roles = ...)]`.

Two roles exist: `Monitor` (admin) and `Cyclist` (regular user), defined in `Static/RoleStatic.cs`.

**Configuration:** `Settings.cs` holds `Secret` (JWT signing key) and `TokenExpirationHours`, populated from `appsettings.json` at startup in `Program.cs`.

**Static files:** Images live in `Resources/Images/` and are served at `/Resources`. The path is configured via `ResourcesImagesPath` in `appsettings.json`.

## NuGet Packages

| Package | Version | Purpose |
|---|---|---|
| `Microsoft.AspNetCore.Authentication.JwtBearer` | 9.0.1 | JWT Bearer token authentication |
| `Microsoft.EntityFrameworkCore.SqlServer` | 9.0.1 | EF Core SQL Server database provider |
| `Microsoft.EntityFrameworkCore.Design` | 9.0.1 | EF Core tooling support (migrations CLI) |
| `Microsoft.EntityFrameworkCore.InMemory` | 9.0.1 | In-memory database provider |
| `Swashbuckle.AspNetCore` | 10.1.0 | Swagger/OpenAPI documentation |
