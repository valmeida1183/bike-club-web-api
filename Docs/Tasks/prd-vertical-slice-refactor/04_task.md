# Task 4.0: Endpoint Auto-Registration, FluentValidation & Exception Handling Wiring

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Wire up the three remaining pieces of foundation: (1) reflection-based `IEndpoint` discovery and mapping, (2) FluentValidation validator auto-registration, and (3) `ProblemDetails` + `IExceptionHandler` replacements for the legacy static `ExceptionHandlerService`. After this task, any new `IEndpoint` added under `Features/` is auto-registered and mapped, and any `DbUpdateException` or `SqlException` (unique/concurrency) produces an RFC 7807 `ProblemDetails` response. Legacy controllers continue to work in parallel.

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
  - `AddGlobalExceptionHandling(this IServiceCollection)` → `AddProblemDetails()` + `AddExceptionHandler<UniqueConstraintExceptionHandler>()` + `AddExceptionHandler<ConcurrencyExceptionHandler>()` + `AddExceptionHandler<GlobalExceptionHandler>()` **in that order** (handlers run in registration order).
  - `UseGlobalExceptionHandling(this WebApplication app)` → `app.UseExceptionHandler()`.
- Create `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` implementing `IExceptionHandler`:
  - `TryHandleAsync` returns `true` only when the exception is `DbUpdateException` whose inner `SqlException.Number` is `2601` or `2627` (mirroring the constants in the existing `ExceptionHandlerService`).
  - Writes a 409 `ProblemDetails` with `title = "Cannot create a record that already exists."`.
- Create `Infrastructure/ExceptionHandlers/ConcurrencyExceptionHandler.cs` for `DbUpdateConcurrencyException` → 400 `ProblemDetails` with `title = "This record is already updated."`.
- Create `Infrastructure/ExceptionHandlers/GlobalExceptionHandler.cs` — catches everything else, writes a 500 `ProblemDetails`, logs at `Error` via `ILogger<GlobalExceptionHandler>`.
- Register `AddEndpoints`, `AddFluentValidation`, and `AddGlobalExceptionHandling` in `Program.cs` (via the extensions introduced in task 3.0 — no inline `builder.Services.Add*` calls).
- Call `app.UseGlobalExceptionHandling()` and `app.MapEndpoints()` in the middleware pipeline. `MapEndpoints()` must come **before** `app.MapControllers()` so that when feature slices start registering endpoints they coexist cleanly.
- Do **not** touch any controller. Controllers continue to use the legacy `ExceptionHandlerService` until the feature is migrated; both paths must coexist until task 13.0 deletes the legacy service.
</requirements>

## Subtasks

- [ ] 4.1 Create `Extensions/EndpointExtensions.cs` per the `minimal-api` skill template.
- [ ] 4.2 Create `Extensions/ValidationExtensions.cs` with `AddFluentValidation`.
- [ ] 4.3 Create `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` (port SQL error codes `2601` / `2627` from `Services/ExceptionHandlerService.cs`).
- [ ] 4.4 Create `ConcurrencyExceptionHandler.cs` and `GlobalExceptionHandler.cs`.
- [ ] 4.5 Create `Extensions/ExceptionHandlingExtensions.cs` and register all three handlers via `AddGlobalExceptionHandling` (registration order matters).
- [ ] 4.6 Wire all three extension groups into `Program.cs`; call `UseGlobalExceptionHandling()` early (before `MapControllers`) and `MapEndpoints()` before `MapControllers()`.
- [ ] 4.7 Manual Verification.

## Implementation Details

See `techspec.md` → "Integration Points → Error contract (RFC 7807 ProblemDetails)" for the status-code mapping, and "Integration Points → Exception handling" for the SQL error-number table. The `minimal-api` skill provides the exact `AddEndpoints`/`MapEndpoints` body.

At the end of this task there are still zero `IEndpoint` implementations in the codebase — `MapEndpoints` iterates an empty sequence. That is expected; feature tasks 5.0–12.0 will populate it.

## Success Criteria

- `dotnet build` succeeds.
- `dotnet watch run` starts; Swagger still lists every existing controller route.
- Forcing a duplicate-key error through a legacy controller (e.g., `POST /v1/accounts/register` with an already-registered email) now produces an **RFC 7807 ProblemDetails** body (status 409, `type`/`title`/`detail` fields) instead of the legacy `{ message: "..." }` shape — because the exception now bubbles out of the controller's `try/catch` via the legacy handler, which still returns the old shape; **OR**, if the legacy handler is bypassed (unhandled path), the new `UniqueConstraintExceptionHandler` produces ProblemDetails. Verify the chain: legacy controllers keep their legacy shape until migrated; **uncaught** exceptions hit the new handlers.
- `IExceptionHandler` middleware is in the pipeline (inspect startup logs or add a deliberate throw in a scratch endpoint to confirm).

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Register a duplicate email via legacy `POST /v1/accounts/register` → still returns legacy `{ message }` shape (legacy controller's try/catch still wraps `ExceptionHandlerService`). This confirms coexistence.
  - [ ] Temporarily add a scratch `IEndpoint` that throws `new Exception("boom")`, hit it, confirm a 500 `ProblemDetails` body is returned (then delete the scratch endpoint).
  - [ ] Confirm `GET /v1/genders` still returns the same payload (Swagger + request).
  - [ ] Confirm JWT-protected routes still require auth.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Extensions/EndpointExtensions.cs` (new)
- `Extensions/ValidationExtensions.cs` (new)
- `Extensions/ExceptionHandlingExtensions.cs` (new)
- `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` (new)
- `Infrastructure/ExceptionHandlers/ConcurrencyExceptionHandler.cs` (new)
- `Infrastructure/ExceptionHandlers/GlobalExceptionHandler.cs` (new)
- `Program.cs` (wire the new extensions)
