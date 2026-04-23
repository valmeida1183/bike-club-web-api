# Technical Specification

**Feature:** Vertical Slice Architecture Refactor
**Related PRD:** `./prd.md`
**Date:** 2026-04-23

## Executive Summary

The refactor replaces the current `Controllers → Services → DataContext → Models` layout with a feature-sliced Minimal API layout. Each public endpoint becomes an independent operation folder holding `Request`, `Response`, `Handler`, `Validator`, and `Endpoint` types. Handlers return `Result`/`Result<T>`; endpoints translate the result into RFC 7807 `ProblemDetails` responses via `TypedResults`. Request validation moves to FluentValidation, invoked inside handlers (so validation failures flow through the same `Result` pipeline as business errors). Startup configuration is extracted to extension methods, and endpoints are discovered via a lightweight reflection-based `AddEndpoints`/`MapEndpoints` pair.

Migration is incremental: the foundation (SharedKernel, Extensions, Domain, Result, FluentValidation wiring, exception handlers, endpoint auto-registration, `Program.cs` shrink) lands first. Then each feature moves over one at a time — Controller deleted only once all its endpoints are live under `Features/`. No route, verb, auth, or success-payload change is introduced.

## System Architecture

### Component Overview

**New components**

- `SharedKernel/IEndpoint.cs` — marker interface (`void MapEndpoint(IEndpointRouteBuilder app)`).
- `SharedKernel/Results/{Result.cs, Error.cs, ValidationResult.cs, ResultExtensions.cs}` — Result pattern core.
- `SharedKernel/Results/ErrorType.cs` — enum (`Failure`, `Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`) used by the endpoint adapter to pick the HTTP status code.
- `SharedKernel/Http/ResultExtensions.cs` — `ToProblemDetails()` / `ToIResult()` helpers that turn a `Result`/`Result<T>` into a `TypedResults.Ok` / `TypedResults.Problem` / `TypedResults.ValidationProblem`.
- `SharedKernel/Services/{TokenService.cs, CryptographerService.cs}` — relocated from root `Services/` (unchanged behavior; now registered as scoped services instead of static classes).
- `SharedKernel/Static/{RoleStatic.cs, GenderStatic.cs}` — relocated from root `Static/`.
- `SharedKernel/Settings.cs` — relocated from root (unchanged; remains a static holder loaded at startup via `LoadSettings`).
- `Extensions/*` — one class per configuration concern (Authentication, Compression, CORS, DataContext, Swagger, LoadSettings, AddEndpoints, AddFluentValidation, AddExceptionHandlers).
- `Extensions/EndpointExtensions.cs` — `AddEndpoints(this IServiceCollection)` (reflection scan) + `MapEndpoints(this WebApplication)`.
- `Infrastructure/ExceptionHandlers/*` — `UniqueConstraintExceptionHandler`, `ConcurrencyExceptionHandler`, `GlobalExceptionHandler` implementing `IExceptionHandler`.
- `Features/<Feature>/<Operation>/*` — per-operation slice files.
- `Features/<Feature>/Shared/*` — optional feature-local shared logic (e.g., `ShopCart.CalculateTotalAmount`).
- `Domain/Entities/*` — EF entities relocated from `Models/`, with all `System.ComponentModel.DataAnnotations` validation attributes removed.

**Modified components**

- `Program.cs` — shrunk to a sequence of extension calls plus middleware pipeline and `app.Run()`.
- `Data/DataContext.cs` — namespace update only; `DbSet<T>` types now resolve to `Domain/Entities/*`. `ApplyConfigurationsFromAssembly(typeof(DataContext).Assembly)` continues to discover the existing `Data/Configurations/*` mappings unchanged.
- `bike-club-api.csproj` — add `FluentValidation.DependencyInjectionExtensions`. Scrutor is not adopted (see Decisions).
- `CLAUDE.md` — updated architecture/commands guidance.

**Data flow**

`HTTP request → Endpoint (binds inputs, builds Request record) → Handler (validator → business logic → DataContext) → Result/Result<T> → ResultExtensions.ToIResult() → TypedResults.Ok | Problem | ValidationProblem`. Unhandled EF/SQL exceptions are caught by `IExceptionHandler` implementations and turned into `ProblemDetails` with the same envelope.

## Implementation Design

### Main Interfaces

```csharp
// SharedKernel/IEndpoint.cs
public interface IEndpoint
{
    void MapEndpoint(IEndpointRouteBuilder app);
}

// SharedKernel/Results/Error.cs
public enum ErrorType { Failure, Validation, NotFound, Conflict, Unauthorized, Forbidden }

public record Error(string Code, string Description, ErrorType Type = ErrorType.Failure)
{
    public static readonly Error None = new(string.Empty, string.Empty);
    public static readonly Error NullValue = new("Error.NullValue", "A null value was provided");
}

// SharedKernel/Results/Result.cs  (contract — see result-pattern skill for full body)
public class Result
{
    public bool IsSuccess { get; }
    public bool IsFailure => !IsSuccess;
    public Error Error { get; }
    public static Result Success();
    public static Result Failure(Error error);
    public static Result<T> Success<T>(T value);
    public static Result<T> Failure<T>(Error error);
}

// SharedKernel/Http/ResultExtensions.cs
public static class ResultExtensions
{
    public static IResult ToIResult(this Result result);
    public static IResult ToIResult<T>(this Result<T> result, Func<T, IResult>? onSuccess = null);
}
```

Handlers follow a uniform contract per operation:

```csharp
internal sealed class CreateAddressHandler
{
    private readonly DataContext _db;
    private readonly IValidator<CreateAddressRequest> _validator;

    public CreateAddressHandler(DataContext db, IValidator<CreateAddressRequest> validator)
    { _db = db; _validator = validator; }

    public async Task<Result<CreateAddressResponse>> Handle(
        CreateAddressRequest request, CancellationToken ct)
    {
        var validation = await _validator.ValidateAsync(request, ct);
        if (!validation.IsValid)
            return ValidationResult<CreateAddressResponse>.WithErrors(
                validation.Errors.Select(e => new Error(e.PropertyName, e.ErrorMessage, ErrorType.Validation)).ToArray());

        var entity = request.ToEntity();
        _db.Addresses.Add(entity);
        await _db.SaveChangesAsync(ct);
        return CreateAddressResponse.From(entity);
    }
}
```

Endpoints stay thin:

```csharp
internal sealed class CreateAddressEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/addresses", async (
                CreateAddressRequest request,
                CreateAddressHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(request, ct)).ToIResult())
           .RequireAuthorization()
           .WithTags("Address");
}
```

### Data Models

- **Entities** (unchanged schema): `Address`, `Bike`, `Category`, `Difficulty`, `Gender`, `Purchase`, `Role`, `ShopCart`, `Tour`, `TourParticipant`, `User`. Moved to `Domain/Entities/`; validation annotations removed.
- **Requests/Responses**: `record`s scoped to a single operation (e.g., `CreateAddressRequest(string Street, string Complement, string State, string City, int ZipCode)` / `CreateAddressResponse(int Id, …)`). Responses intentionally mirror the current success payload shapes so clients are unaffected.
- **Result types**: `Result`, `Result<T>`, `ValidationResult`, `ValidationResult<T>`, `Error`.

### API Endpoints

Full `/v1/` surface is preserved. Summary (path, verb, auth role):

- `v1/accounts/login` (POST, anon), `v1/accounts/register` (POST, anon).
- `v1/addresses` (GET auth, POST auth, PUT/{id} auth, DELETE/{id} Monitor).
- `v1/bikes` (GET auth + filters, GET/{id} auth, POST Monitor, PUT/{id} Monitor, DELETE/{id} Monitor).
- `v1/categories` (GET anon, GET/{id} anon, POST Monitor, PUT/{id} Monitor, DELETE/{id} Monitor).
- `v1/difficulties` (GET anon, GET/{id} anon, POST Monitor, PUT/{id} Monitor, DELETE/{id} Monitor).
- `v1/genders` (GET anon, GET/{code} anon, POST Monitor).
- `v1/purchases` (POST auth, DELETE/{id} auth).
- `v1/roles` (GET auth, GET/{name} auth, POST Monitor, PUT/{name} Monitor).
- `v1/shop-carts` (GET/{userId}, POST, POST/add-purchase, PATCH/update-purchase/{shopCartId}/{bikeId}, PATCH/{shopCartId}/address, DELETE/remove-purchase/{shopCartId}/{bikeId} — all `auth`).
- `v1/tours` (GET auth, GET/{id} auth, POST/PUT/DELETE Monitor).
- `v1/users` (GET auth, GET/{id} auth, POST Monitor, PUT/{id} Monitor).

Each becomes one `IEndpoint` class under `Features/<Feature>/<Operation>/`. Success payloads are preserved exactly; error bodies are standardized (see Integration Points).

## Integration Points

- **Authentication**: continues to use `AddAuthentication().AddJwtBearer(...)` configured in `Extensions/AuthenticationExtensions.cs`. Endpoints use `.RequireAuthorization()` or `.RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })`.
- **Persistence**: `DataContext` is injected directly into handlers as scoped. EF `IEntityTypeConfiguration<T>` mappings and seed data remain untouched.
- **Static files**: `Resources/` stays at the project root — the `.csproj` `Content Include="Resources\**"` rule and `UseStaticFiles` with `PhysicalFileProvider` require it there; this is preserved.
- **Error contract (RFC 7807 ProblemDetails)**: `builder.Services.AddProblemDetails()` is configured in `Extensions/ExceptionHandlingExtensions.cs`. `ResultExtensions.ToIResult` maps `ErrorType` to status:
  - `Validation` → `TypedResults.ValidationProblem(errorsDict)` (400, includes per-property errors).
  - `NotFound` → `TypedResults.Problem(statusCode: 404, title: Error.Code, detail: Error.Description)`.
  - `Conflict` → `TypedResults.Problem(statusCode: 409, …)`.
  - `Unauthorized` → `TypedResults.Problem(statusCode: 401, …)`.
  - `Forbidden` → `TypedResults.Problem(statusCode: 403, …)`.
  - `Failure` (default) → `TypedResults.Problem(statusCode: 400, …)`.
  Success `Result<T>` → `TypedResults.Ok(result.Value)` by default, or the caller-supplied `onSuccess` factory (for `Created` etc.).
- **Exception handling**: `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler` inspects `DbUpdateException` for SQL error numbers 2601/2627 (preserving the current `ExceptionHandlerService` logic) and writes a 409 `ProblemDetails`. `ConcurrencyExceptionHandler` handles `DbUpdateConcurrencyException` as 400. `GlobalExceptionHandler` covers the rest as 500. All are registered via `services.AddExceptionHandler<T>()` + `app.UseExceptionHandler()`; the old static `ExceptionHandlerService` is deleted after the last feature migrates.
- **FluentValidation registration**: `AddValidatorsFromAssemblyContaining<Program>()` in `Extensions/ValidationExtensions.cs`. Validators are scoped. Handlers resolve them via constructor injection as `IValidator<TRequest>`.
- **Endpoint auto-registration**: `Extensions/EndpointExtensions.cs` scans `typeof(Program).Assembly` for non-abstract types implementing `IEndpoint`, registers each as `AddSingleton(typeof(IEndpoint), t)`. `MapEndpoints(this WebApplication app)` resolves `IEnumerable<IEndpoint>` and invokes `MapEndpoint(app)` on each. Scrutor is not used (see Decisions).

## Testing Approach

Per the PRD, automated tests are out of scope. Verification is manual.

### Unit Tests

Not introduced in this refactor.

### Integration Tests

Not introduced. Manual verification steps per feature slice:

- Run `dotnet watch run`, open `https://localhost:5001/swagger`.
- For each migrated feature, execute one representative request per verb (happy path + one validation failure + one auth-denied scenario).
- Confirm status code, success payload shape, and — for failures — the new `ProblemDetails` envelope.

## Development Sequencing

### Build Order

1. **Scaffolding (foundation PR)** — create `SharedKernel/`, `Extensions/`, `Domain/`, `Infrastructure/ExceptionHandlers/`; add `IEndpoint`, Result/Error/ValidationResult/ResultExtensions, `ErrorType`, `ToIResult`. Introduce FluentValidation package. Extract `Program.cs` setup into extension methods. Register `AddProblemDetails`, `AddExceptionHandler<T>()`, `AddEndpoints`, `AddValidatorsFromAssemblyContaining<Program>`. Move `Settings`, `Static/*`, `TokenService`, `CryptographerService` into `SharedKernel/`. Move `Models/*` → `Domain/Entities/*` and update `DataContext` namespace references. Do **not** remove controllers or data annotations yet — app must still build and serve the existing routes.
2. **Account** (proves login/register + JWT + unique-email conflict end-to-end).
3. **Address** (simplest CRUD — smoke test the CRUD pattern and delete-controller cutover).
4. **Lookups** — **Gender**, **Category**, **Difficulty**, **Role** (three of four have anonymous GETs with `ResponseCache`, which must be preserved via `.CacheOutput()` or `[ResponseCache]` equivalent; see Risks).
5. **Bike** (filtering via `[FromQuery]`, includes).
6. **Tour** (straight CRUD, Monitor-only writes).
7. **User** (Monitor-only admin creation, password hashing, no password-change endpoint today).
8. **Purchase**.
9. **ShopCart** (most complex — includes feature-local shared logic `CalculateTotalAmount` that lives in `Features/ShopCart/Shared/`).
10. **Cleanup PR** — delete `Controllers/`, delete `Services/ExceptionHandlerService.cs`, delete old `Services/` folder once empty, remove all validation `DataAnnotations` from `Domain/Entities/*`, update `CLAUDE.md`.

Each feature PR must: (a) delete the corresponding controller and (b) run smoke tests against every endpoint in that feature.

### Technical Dependencies

- `FluentValidation` + `FluentValidation.DependencyInjectionExtensions` (new NuGet, compatible with .NET 9).
- No framework version change. No new infrastructure. SQL Server connection string unchanged.

## Monitoring and Observability

The project has no existing metrics/Grafana integration; this refactor introduces none. Logging guidance:

- `IExceptionHandler` implementations log via `ILogger<T>` at `Error` for 5xx and at `Warning` for known 4xx DB errors (unique violation, concurrency) before writing the ProblemDetails response.
- Handlers do not log validation failures (4xx noise avoidance).
- ASP.NET Core request logging and Swagger remain as today.

## Technical Considerations

### Main Decisions

1. **Per-endpoint full paths, no `MapGroup`.** Each endpoint class declares its own `v1/<resource>` path and `RequireAuthorization(...)`. Simpler, self-contained slices; no cross-file coupling. Rejected: feature-level `MapGroup` — requires a shared per-feature bootstrap class, weakening slice independence.
2. **Hand-rolled reflection for `AddEndpoints`.** No extra dependency, trivial implementation, matches the `minimal-api` skill template verbatim. Rejected: **Scrutor** (extra dep for a one-line scan; value is low here) and **source generators** (overkill, introduces tooling complexity).
3. **RFC 7807 ProblemDetails as the unified error envelope.** Standards-compliant, first-class in ASP.NET Core 9 via `TypedResults.Problem` / `TypedResults.ValidationProblem` and `AddProblemDetails`. Rejected: custom JSON envelope — locks us out of framework helpers and Swagger ProblemDetails schemas. Rejected: hybrid — complexity without clear client benefit.
4. **Validation invoked inside handlers, not via an endpoint filter.** Matches the project prompt and keeps validation failures on the same `Result` rail as business failures. Trade-off: each handler constructor takes an `IValidator<TRequest>` (mitigated by DI auto-registration).
5. **IExceptionHandler + ProblemDetails for DB errors.** Replaces the static `ExceptionHandlerService` and lets EF/SQL exceptions map to the same envelope shape as `Result.Failure`. Rejected: try/catch inside every handler — boilerplate explosion.
6. **`Resources/` stays at the project root.** Constrained by `.csproj` `Content Include` + static-files middleware. SharedKernel hosts only the code; physical asset folder stays put.
7. **TokenService/CryptographerService become instance services.** Enables proper DI testing and avoids `Settings` static coupling inside `TokenService`. Interface + scoped implementation.

### Known Risks

- **`ResponseCache` on anonymous lookup GETs.** `GenderController`, `CategoryController`, `DifficultyController` decorate `Get` with `[ResponseCache(VaryByHeader = "User-Agent", Location = Any, Duration = 30)]`. Minimal APIs need `app.UseOutputCache()` + `.CacheOutput(b => b.Expire(...))` or equivalent headers written manually. Mitigation: introduce Output Caching in the foundation PR; port each lookup endpoint with `.CacheOutput(...)`.
- **Error response shape is a breaking change for clients.** Today's error body is `{ message: "..." }` or `ModelState` dictionary; post-refactor it is RFC 7807. Mitigation: documented in the PRD's "Out of Scope" caveat; announce to API consumers before the cleanup PR.
- **Auto-registration picks up stale endpoint classes.** Any `IEndpoint` implementor anywhere in the assembly is mapped. Mitigation: keep `IEndpoint` non-public-only by convention (internal sealed) and avoid leaving orphans during migration.
- **Silent 200 on missing records.** Current `GetById` controllers `return Ok(null)` when not found. The refactor will switch these to `Result.Failure(ErrorType.NotFound)` → 404. This is a client-visible behavioral improvement but worth calling out.
- **`TourParticipant` has no controller today** but exists as a DbSet. It stays a pure domain entity without a feature folder — no slice created.

### Compliance with Standard Skills

- **`result-pattern` skill** — followed for `Result`/`Result<T>`/`Error`/`ValidationResult`/`ResultExtensions` shapes. Addition: `ErrorType` enum on `Error` (skill doesn't prescribe a category field; we need it so the endpoint adapter can pick an HTTP status without string-matching `Code`). Justification: avoids controller-style `switch (error.Code) { ... }` at every call site.
- **`minimal-api` skill** — followed for `IEndpoint`, `AddEndpoints`/`MapEndpoints`, reflection-based scan, per-feature folder structure. Placement under `Extensions/` matches the skill's suggested location.
- **`create-prd`/`create-techspec`/`create-tasks` skills** — authoring workflow compliance (this document). Tasks will be produced next via `create-tasks`.

### Relevant and Dependent Files

- `Program.cs`, `Settings.cs`, `bike-club-api.csproj`, `appsettings.json`, `CLAUDE.md`.
- `Controllers/*.cs` (11 files — all deleted after migration).
- `Services/TokenService.cs`, `Services/CryptographerSerivce.cs` (note existing typo in filename — will be corrected to `CryptographerService.cs` on move), `Services/ExceptionHandlerService.cs` (deleted).
- `Static/RoleStatic.cs`, `Static/GenderStatic.cs` (moved).
- `Models/*.cs` (11 files — moved to `Domain/Entities/`, validation attributes stripped).
- `Data/DataContext.cs`, `Data/Configurations/*.cs`, `Data/Seed/*.cs`, `Data/Extensions/ModelBuilderExtensions.cs` (namespace updates only).
- `Resources/Images/**` (untouched).
