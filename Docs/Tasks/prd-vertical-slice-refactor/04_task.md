# Task 4.0: Endpoint Auto-Registration, FluentValidation & Exception Handling Wiring

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Wire up the three remaining pieces of foundation: (1) reflection-based `IEndpoint` discovery and mapping, (2) FluentValidation validator auto-registration, and (3) `IExceptionHandler` replacements for the legacy static `ExceptionHandlerService`. After this task, any new `IEndpoint` added under `Features/` is auto-registered and mapped, and any `DbUpdateException` or `SqlException` (unique/concurrency) produces a `Result.Failure`-shaped response body — the same envelope handlers return — so clients parse one shape regardless of error source. **`ProblemDetails` is not used** anywhere in the new pipeline. Legacy controllers continue to work in parallel.

<skills>
### Compliance with Standard Skills

- **`minimal-api`** — follow the skill's `AddEndpoints`/`MapEndpoints` reflection template in `Extensions/EndpointExtensions.cs`.
</skills>

<requirements>
- Create `Extensions/EndpointExtensions.cs` with:
  - `AddEndpoints(this IServiceCollection)` — scans `typeof(Program).Assembly` for non-abstract, non-interface types implementing `IEndpoint`, registers each as `AddSingleton(typeof(IEndpoint), t)`.
  - `MapEndpoints(this WebApplication app, RouteGroupBuilder? routeGroupBuilder = null)` — resolves `IEnumerable<IEndpoint>` and invokes `MapEndpoint(app)` on each.
  - Optional: `HasPermission(this RouteHandlerBuilder, string)` helper matching the skill template (unused today but available for future role checks).
- Create `Extensions/ValidationExtensions.cs` with `AddFluentValidation(this IServiceCollection)` → `services.AddValidatorsFromAssemblyContaining<Program>()` (scoped lifetime is the default).
- Create `Extensions/ExceptionHandlingExtensions.cs` with:
  - `AddGlobalExceptionHandling(this IServiceCollection)` → `AddExceptionHandler<UniqueConstraintExceptionHandler>()` + `AddExceptionHandler<ConcurrencyExceptionHandler>()` + `AddExceptionHandler<GlobalExceptionHandler>()` **in that order** (handlers run in registration order). **Do NOT** call `AddProblemDetails()` — the new pipeline does not produce ProblemDetails.
  - `UseGlobalExceptionHandling(this WebApplication app)` → `app.UseExceptionHandler()`.
- Create `Infrastructure/ExceptionHandlers/InfrastructureErrors.cs` — canonical `Error` instances reused by the handler chain: `UniqueConstraint` (`Error.Type = Conflict`, code `Db.UniqueConstraint`, description `"Cannot create a record that already exists."`), `Concurrency` (`Validation`, `Db.Concurrency`, `"This record is already updated."`), `Unexpected` (`Failure`, `Server.Unexpected`, `"An unexpected error occurred."`).
- Create `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` implementing `IExceptionHandler`:
  - `TryHandleAsync` returns `true` only when the exception is `DbUpdateException` whose inner `SqlException.Number` is `2601` or `2627` (mirroring the constants in the existing `ExceptionHandlerService`).
  - Sets `httpContext.Response.StatusCode = StatusCodes.Status409Conflict` and writes `Result.Failure(InfrastructureErrors.UniqueConstraint)` to the response body via `httpContext.Response.WriteAsJsonAsync(...)`.
- Create `Infrastructure/ExceptionHandlers/ConcurrencyExceptionHandler.cs` for `DbUpdateConcurrencyException` → status 400, body `Result.Failure(InfrastructureErrors.Concurrency)`.
- Create `Infrastructure/ExceptionHandlers/GlobalExceptionHandler.cs` — catches everything else, status 500, body `Result.Failure(InfrastructureErrors.Unexpected)`, logs at `Error` via `ILogger<GlobalExceptionHandler>`.
- All three handlers MUST serialize their response using the same JSON options used by minimal API endpoints so the `Result` envelope shape on the wire is byte-identical to handler-returned failures.
- Register `AddEndpoints`, `AddFluentValidation`, and `AddGlobalExceptionHandling` in `Program.cs` (via the extensions introduced in task 3.0 — no inline `builder.Services.Add*` calls).
- Call `app.UseGlobalExceptionHandling()` and `app.MapEndpoints()` in the middleware pipeline. `MapEndpoints()` must come **before** `app.MapControllers()` so that when feature slices start registering endpoints they coexist cleanly.
- Do **not** touch any controller. Controllers continue to use the legacy `ExceptionHandlerService` until the feature is migrated; both paths must coexist until task 13.0 deletes the legacy service.
</requirements>

## Subtasks

- [ ] 4.1 Create `Extensions/EndpointExtensions.cs` per the `minimal-api` skill template.
- [ ] 4.2 Create `Extensions/ValidationExtensions.cs` with `AddFluentValidation`.
- [ ] 4.3 Create `Infrastructure/ExceptionHandlers/InfrastructureErrors.cs` (canonical `Error` instances for the handler chain).
- [ ] 4.4 Create `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` (port SQL error codes `2601` / `2627` from `Services/ExceptionHandlerService.cs`; emits `Result.Failure(InfrastructureErrors.UniqueConstraint)` body at status 409).
- [ ] 4.5 Create `ConcurrencyExceptionHandler.cs` (`Result.Failure(InfrastructureErrors.Concurrency)` at 400) and `GlobalExceptionHandler.cs` (`Result.Failure(InfrastructureErrors.Unexpected)` at 500).
- [ ] 4.6 Create `Extensions/ExceptionHandlingExtensions.cs` and register all three handlers via `AddGlobalExceptionHandling` (registration order matters). Do **not** call `AddProblemDetails()`.
- [ ] 4.7 Wire all three extension groups into `Program.cs`; call `UseGlobalExceptionHandling()` early (before `MapControllers`) and `MapEndpoints()` before `MapControllers()`.
- [ ] 4.8 Manual Verification.

## Implementation Details

See `techspec.md` → "Integration Points → Response envelope (Result / Result<T>)" for the status-code mapping, and "Integration Points → Exception handling (envelope-consistent)" for the SQL error-number table. The `minimal-api` skill provides the exact `AddEndpoints`/`MapEndpoints` body.

At the end of this task there are still zero `IEndpoint` implementations in the codebase — `MapEndpoints` iterates an empty sequence. That is expected; feature tasks 5.0–12.0 will populate it.

## Success Criteria

- `dotnet build` succeeds.
- `dotnet watch run` starts; Swagger still lists every existing controller route.
- Legacy controllers keep their legacy `{ message }` error shape until migrated (their `try/catch` still wraps the old `ExceptionHandlerService`).
- **Uncaught** exceptions hit the new handlers and produce a **`Result`-envelope** body — e.g., `{ "isSuccess": false, "isFailure": true, "error": { "code": "Db.UniqueConstraint", "description": "Cannot create a record that already exists.", "type": "Conflict" } }` at status 409 — never `ProblemDetails`.
- `IExceptionHandler` middleware is in the pipeline (inspect startup logs or add a deliberate throw in a scratch endpoint to confirm).

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Register a duplicate email via legacy `POST /v1/accounts/register` → still returns legacy `{ message }` shape (legacy controller's try/catch still wraps `ExceptionHandlerService`). This confirms coexistence.
  - [ ] Temporarily add a scratch `IEndpoint` that throws `new Exception("boom")`, hit it, confirm a 500 response body of shape `{ "isSuccess": false, "isFailure": true, "error": { "code": "Server.Unexpected", ..., "type": "Failure" } }` (Result envelope, not ProblemDetails). Then delete the scratch endpoint.
  - [ ] Temporarily add a scratch `IEndpoint` that triggers a unique-constraint violation (insert a row with a duplicate key), hit it, confirm 409 with `{ "isSuccess": false, "error": { "code": "Db.UniqueConstraint", "type": "Conflict" } }`. Then delete the scratch endpoint.
  - [ ] Confirm `GET /v1/genders` still returns the same payload (Swagger + request).
  - [ ] Confirm JWT-protected routes still require auth.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Extensions/EndpointExtensions.cs` (new)
- `Extensions/ValidationExtensions.cs` (new)
- `Extensions/ExceptionHandlingExtensions.cs` (new)
- `Infrastructure/ExceptionHandlers/InfrastructureErrors.cs` (new — canonical `Error` instances)
- `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` (new)
- `Infrastructure/ExceptionHandlers/ConcurrencyExceptionHandler.cs` (new)
- `Infrastructure/ExceptionHandlers/GlobalExceptionHandler.cs` (new)
- `Program.cs` (wire the new extensions)
