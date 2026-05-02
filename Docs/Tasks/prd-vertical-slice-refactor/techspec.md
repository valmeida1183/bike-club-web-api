# Technical Specification

**Feature:** Vertical Slice Architecture Refactor
**Related PRD:** `./prd.md`
**Date:** 2026-04-23

## Executive Summary

The refactor replaces the current `Controllers → Services → DataContext → Models` layout with a feature-sliced Minimal API layout. Each public endpoint becomes an independent operation folder holding `Request`, `Response`, `Handler`, `Validator`, and `Endpoint` types. Handlers return `Result`/`Result<T>`; endpoints serialize that **Result object itself** as the response body via `Results.Ok(result)` / `Results.BadRequest(result)` / `Results.NotFound(result)` / `Results.Conflict(result)` (status code derived from `Error.Type`). The same envelope is used for failures surfaced by the global exception middleware — no `ProblemDetails`, no bare-value success payloads. Endpoints whose operation conventionally has no response body (success → `204 NoContent`) keep that body-less success path; their failure paths still carry the Result envelope. Request validation moves to FluentValidation, invoked inside handlers (so validation failures flow through the same `Result` pipeline as business errors). Startup configuration is extracted to extension methods, and endpoints are discovered via a lightweight reflection-based `AddEndpoints`/`MapEndpoints` pair.

Migration is incremental: the foundation (SharedKernel, Extensions, Domain, Result, FluentValidation wiring, exception handlers, endpoint auto-registration, `Program.cs` shrink) lands first. Then each feature moves over one at a time — Controller deleted only once all its endpoints are live under `Features/`. No route, verb, auth, or success-status-code change is introduced; the success **body** is wrapped in the `Result<T>` envelope (the previous bare value moves into `Result.Value`).

## System Architecture

### Component Overview

**New components**

- `SharedKernel/IEndpoint.cs` — marker interface (`void MapEndpoint(IEndpointRouteBuilder app)`).
- `SharedKernel/Results/{Result.cs, Error.cs, ValidationResult.cs, ResultExtensions.cs}` — Result pattern core.
- `SharedKernel/Results/ErrorType.cs` — enum (`Failure`, `Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`) used by the endpoint adapter to pick the HTTP status code.
- `SharedKernel/Http/ResultExtensions.cs` — `ToIResult()` helpers that turn a `Result`/`Result<T>` into `Results.Ok(result)` / `Results.BadRequest(result)` / `Results.NotFound(result)` / `Results.Conflict(result)` / `Results.Unauthorized()` / `Results.Forbid()`. The Result object itself is the response body (no `ProblemDetails`, no value-unwrapping). Status code is selected from `Error.Type`. An optional `onSuccess` factory lets callers override the success branch (e.g., to return `Results.Created(uri, result)` instead of `Results.Ok(result)`); when omitted, success defaults to `Results.Ok(result)` for `Result<T>` and `Results.NoContent()` for non-generic `Result`.
- `SharedKernel/Services/{TokenService.cs, CryptographerService.cs}` — relocated from root `Services/` (unchanged behavior; now registered as scoped services instead of static classes).
- `SharedKernel/Static/{RoleStatic.cs, GenderStatic.cs}` — relocated from root `Static/`.
- `SharedKernel/Settings.cs` — relocated from root (unchanged; remains a static holder loaded at startup via `LoadSettings`).
- `Extensions/*` — one class per configuration concern (Authentication, Compression, CORS, DataContext, Swagger, LoadSettings, AddEndpoints, AddHandlers, AddFluentValidation, AddExceptionHandlers).
- `Extensions/EndpointExtensions.cs` — `AddEndpoints(this IServiceCollection)` (reflection scan for `IEndpoint` types) + `MapEndpoints(this WebApplication)`.
- `Extensions/HandlerExtensions.cs` — `AddHandlers(this IServiceCollection)` scans `BikeClub.Features.*` for non-abstract types whose name ends in `Handler` and registers each as `AddScoped`.
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

`HTTP request → Endpoint (binds inputs, builds Request record) → Handler (validator → business logic → DataContext) → Result/Result<T> → ResultExtensions.ToIResult() → Results.Ok(result) | Results.BadRequest(result) | Results.NotFound(result) | Results.Conflict(result) | Results.NoContent() (body-less success only) | Results.Unauthorized() | Results.Forbid()`. The Result object is serialized as the response body wherever the chosen status helper accepts one. Unhandled EF/SQL exceptions are caught by `IExceptionHandler` implementations and converted into a `Result.Failure` with the appropriate `Error.Type` so the wire response is the same `Result`-envelope shape as handler-returned failures (no `ProblemDetails`).

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
//
// The Result object itself is the response body for any helper that accepts one.
// Unauthorized/Forbid have no body by framework design; status alone signals the outcome.
//
// Status mapping (failures):
//   Validation   -> Results.BadRequest(result)        // 400
//   NotFound     -> Results.NotFound(result)          // 404
//   Conflict     -> Results.Conflict(result)          // 409
//   Unauthorized -> Results.Unauthorized()            // 401, no body
//   Forbidden    -> Results.Forbid()                  // 403, no body
//   Failure      -> Results.BadRequest(result)        // 400 (default)
//
// Status mapping (success):
//   Result<T>.Success    -> Results.Ok(result)        // 200, body = full Result<T>
//   Result.Success       -> Results.NoContent()       // 204, no body, by default
//                          (override via onSuccess to return Ok(result), Created, etc.)
public static class ResultExtensions
{
    // Non-generic Result: success defaults to NoContent(); override via onSuccess to attach a body.
    public static IResult ToIResult(this Result result, Func<Result, IResult>? onSuccess = null);

    // Result<T>: success defaults to Ok(result) — body is the entire Result<T> object, value lives at result.Value.
    public static IResult ToIResult<T>(this Result<T> result, Func<Result<T>, IResult>? onSuccess = null);
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

Endpoints stay thin. The `ToIResult()` helper picks the status code from `Error.Type` on failure and serializes the entire `Result`/`Result<T>` object as the response body:

```csharp
// Result<T> handler — body on every response (success and failure)
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

// Non-generic Result handler — success returns 204 NoContent (no body), failure returns the Result envelope
internal sealed class DeletePurchaseEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapDelete("v1/purchases/{id:int}", async (
                int id,
                DeletePurchaseHandler handler,
                CancellationToken ct) =>
            (await handler.Handle(id, ct)).ToIResult())
           .RequireAuthorization()
           .WithTags("Purchase");
}

// Non-generic Result handler that wants Ok(result) on success instead of NoContent
internal sealed class SomeOperationEndpoint : IEndpoint
{
    public void MapEndpoint(IEndpointRouteBuilder app) =>
        app.MapPost("v1/something", async (...handler, ct) =>
            (await handler.Handle(...)).ToIResult(onSuccess: r => Results.Ok(r)));
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

Each becomes one `IEndpoint` class under `Features/<Feature>/<Operation>/`. Success **values** are preserved exactly (the data the client receives is the same), but they now live inside the `Result<T>.Value` property of the response envelope rather than at the body root; error bodies use the same `Result` envelope (see Integration Points → Response envelope).

## Integration Points

- **Authentication**: continues to use `AddAuthentication().AddJwtBearer(...)` configured in `Extensions/AuthenticationExtensions.cs`. Endpoints use `.RequireAuthorization()` or `.RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })`.
- **Persistence**: `DataContext` is injected directly into handlers as scoped. EF `IEntityTypeConfiguration<T>` mappings and seed data remain untouched.
- **Static files**: `Resources/` stays at the project root — the `.csproj` `Content Include="Resources\**"` rule and `UseStaticFiles` with `PhysicalFileProvider` require it there; this is preserved.
- **Response envelope (Result / Result<T>)**: `ProblemDetails` is **not** used. `ResultExtensions.ToIResult` serializes the `Result`/`Result<T>` object itself as the response body and selects the status helper from `Error.Type`:
  - `Validation` → `Results.BadRequest(result)` (400, body is the `ValidationResult`/`ValidationResult<T>` with the `Errors[]` array).
  - `NotFound` → `Results.NotFound(result)` (404).
  - `Conflict` → `Results.Conflict(result)` (409).
  - `Unauthorized` → `Results.Unauthorized()` (401, no body — framework helper does not accept one).
  - `Forbidden` → `Results.Forbid()` (403, no body — framework helper does not accept one).
  - `Failure` (default) → `Results.BadRequest(result)` (400).
  - `Result<T>.Success` → `Results.Ok(result)` (200, body = the entire `Result<T>` object; the success payload is at `result.Value`). Caller may override via `onSuccess` (e.g., `Results.Created(uri, result)`).
  - `Result.Success` (non-generic) → `Results.NoContent()` (204, no body) by default. Caller may override via `onSuccess` to return `Results.Ok(result)` if a body is desired (e.g., to convey `IsSuccess: true` explicitly).
- **Exception handling (envelope-consistent)**: `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler` inspects `DbUpdateException` for SQL error numbers 2601/2627 (preserving the current `ExceptionHandlerService` logic) and writes a 409 with a `Result.Failure(InfrastructureErrors.UniqueConstraint)` (`ErrorType.Conflict`) body — same envelope shape as a handler-returned failure. `ConcurrencyExceptionHandler` handles `DbUpdateConcurrencyException` as 400 with `Result.Failure(InfrastructureErrors.Concurrency)`. `GlobalExceptionHandler` covers the rest as 500 with `Result.Failure(InfrastructureErrors.Unexpected)`. All are registered via `services.AddExceptionHandler<T>()` + `app.UseExceptionHandler()`. `Infrastructure/ExceptionHandlers/InfrastructureErrors.cs` defines the canonical `Error` instances. `AddProblemDetails()` is **not** registered. The old static `ExceptionHandlerService` is deleted after the last feature migrates.
- **FluentValidation registration**: `AddValidatorsFromAssemblyContaining<Program>()` in `Extensions/ValidationExtensions.cs`. Validators are scoped. Handlers resolve them via constructor injection as `IValidator<TRequest>`.
- **Endpoint auto-registration**: `Extensions/EndpointExtensions.cs` scans `typeof(Program).Assembly` for non-abstract types implementing `IEndpoint`, registers each as `AddSingleton(typeof(IEndpoint), t)`. `MapEndpoints(this WebApplication app)` resolves `IEnumerable<IEndpoint>` and invokes `MapEndpoint(app)` on each. Scrutor is not used (see Decisions).
- **Handler auto-registration**: `Extensions/HandlerExtensions.cs` (`AddHandlers`) scans `BikeClub.Features.*` for non-abstract types whose name ends in `Handler` and registers each as `AddScoped`. The `BikeClub.Features.` namespace prefix (with trailing dot) prevents accidental pickup of infrastructure types such as the `IExceptionHandler` implementations in `Infrastructure/ExceptionHandlers/`.

## Testing Approach

Per the PRD, automated tests are out of scope. Verification is manual.

### Unit Tests

Not introduced in this refactor.

### Integration Tests

Not introduced. Manual verification steps per feature slice:

- Run `dotnet watch run`, open `https://localhost:5001/swagger`.
- For each migrated feature, execute one representative request per verb (happy path + one validation failure + one auth-denied scenario).
- Confirm status code, success-value shape (read from `response.value` of the `Result<T>` envelope), and — for failures — the same envelope with `response.error` populated (`code`, `description`, `type`).

## Development Sequencing

### Build Order

1. **Scaffolding (foundation PR)** — create `SharedKernel/`, `Extensions/`, `Domain/`, `Infrastructure/ExceptionHandlers/`; add `IEndpoint`, Result/Error/ValidationResult/ResultExtensions, `ErrorType`, `ToIResult`. Introduce FluentValidation package. Extract `Program.cs` setup into extension methods. Register `AddExceptionHandler<T>()` (Unique/Concurrency/Global), `AddEndpoints`, `AddValidatorsFromAssemblyContaining<Program>`. **Do NOT register `AddProblemDetails()`** — the new pipeline produces Result-envelope responses, not ProblemDetails. Move `Settings`, `Static/*`, `TokenService`, `CryptographerService` into `SharedKernel/`. Move `Models/*` → `Domain/Entities/*` and update `DataContext` namespace references. Do **not** remove controllers or data annotations yet — app must still build and serve the existing routes.
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

- `IExceptionHandler` implementations log via `ILogger<T>` at `Error` for 5xx and at `Warning` for known 4xx DB errors (unique violation, concurrency) before writing the `Result.Failure` envelope response (status code + JSON body).
- Handlers do not log validation failures (4xx noise avoidance).
- ASP.NET Core request logging and Swagger remain as today.

## Technical Considerations

### Main Decisions

1. **Per-endpoint full paths, no `MapGroup`.** Each endpoint class declares its own `v1/<resource>` path and `RequireAuthorization(...)`. Simpler, self-contained slices; no cross-file coupling. Rejected: feature-level `MapGroup` — requires a shared per-feature bootstrap class, weakening slice independence.
2. **Hand-rolled reflection for `AddEndpoints`.** No extra dependency, trivial implementation, matches the `minimal-api` skill template verbatim. Rejected: **Scrutor** (extra dep for a one-line scan; value is low here) and **source generators** (overkill, introduces tooling complexity).
3. **Result/Result<T> object as the unified response envelope (success and failure).** The `Result` object the handler returns is serialized verbatim as the HTTP body via `Results.Ok(result)` / `Results.BadRequest(result)` / `Results.NotFound(result)` / `Results.Conflict(result)`; the `Error.Type` enum drives status code selection. Same envelope is reused by the `IExceptionHandler` chain so unexpected exceptions surface as `Result.Failure` bodies, not `ProblemDetails`. Rationale: clients parse one shape regardless of source (handler outcome or infrastructure failure), debugging is uniform, and the wire contract aligns directly with the in-process `Result` API. **Trade-offs accepted**: (a) breaks today's bare-value success bodies — the previous payload now lives at `response.value`; (b) abandons ASP.NET Core's first-class `TypedResults.Problem`/`ValidationProblem` helpers and the Swagger ProblemDetails schemas; (c) status-helper limitations: `Results.Unauthorized()` and `Results.Forbid()` do not accept a body, so 401/403 responses carry status only. Rejected: **RFC 7807 ProblemDetails** — would force two parsing branches on the client (envelope vs ProblemDetails); contradicts the explicit user requirement that endpoints return `Results.Ok(result)` / `Results.BadRequest(result)`. Rejected: **bare-value success + ProblemDetails errors** (the prior plan) — same two-shape problem and inconsistent with the handler API. Rejected: **hybrid** — complexity without client benefit.
4. **Validation invoked inside handlers, not via an endpoint filter.** Matches the project prompt and keeps validation failures on the same `Result` rail as business failures. Trade-off: each handler constructor takes an `IValidator<TRequest>` (mitigated by DI auto-registration).
5. **IExceptionHandler emits the Result envelope for DB / unhandled errors.** Replaces the static `ExceptionHandlerService` and lets EF/SQL exceptions surface as `Result.Failure(Error)` bodies — exactly the same envelope shape handlers return for explicit failures. The chain is `UniqueConstraintExceptionHandler` (409 / `ErrorType.Conflict`) → `ConcurrencyExceptionHandler` (400 / `ErrorType.Validation`) → `GlobalExceptionHandler` (500 / `ErrorType.Failure`). Rejected: **ProblemDetails for DB errors** — would split clients across two response shapes (envelope for handler outcomes, ProblemDetails for infrastructure errors). Rejected: **try/catch inside every handler** — boilerplate explosion.
6. **`Resources/` stays at the project root.** Constrained by `.csproj` `Content Include` + static-files middleware. SharedKernel hosts only the code; physical asset folder stays put.
7. **TokenService/CryptographerService become instance services.** Enables proper DI testing and avoids `Settings` static coupling inside `TokenService`. Interface + scoped implementation.

### Known Risks

- **`ResponseCache` on anonymous lookup GETs.** `GenderController`, `CategoryController`, `DifficultyController` decorate `Get` with `[ResponseCache(VaryByHeader = "User-Agent", Location = Any, Duration = 30)]`. Minimal APIs need `app.UseOutputCache()` + `.CacheOutput(b => b.Expire(...))` or equivalent headers written manually. Mitigation: introduce Output Caching in the foundation PR; port each lookup endpoint with `.CacheOutput(...)`.
- **Response shape is a breaking change for clients on both success and failure paths.** Today's success body is the bare value (e.g., `[{ id: 1, ... }]`); post-refactor it is `{ isSuccess, isFailure, error: { code, description, type }, value: [{ id: 1, ... }] }`. Today's error body is `{ message: "..." }` or a `ModelState` dictionary; post-refactor it is the same `Result` envelope with `isSuccess: false` and `error` populated. The change is intentional and explicitly approved by the PRD. Mitigation: documented in the PRD's "Out of Scope" caveat; announce to API consumers before the cleanup PR; provide a migration snippet showing `response.value` for success reads. Endpoints whose success path is `204 NoContent` still carry the Result envelope on failures only.
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
