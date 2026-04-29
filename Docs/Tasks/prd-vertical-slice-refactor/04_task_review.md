# Review: Task 4.0 - Endpoint Auto-Registration, FluentValidation & Exception Handling Wiring

**Reviewer**: AI Code Reviewer
**Date**: 2026-04-28
**Task File**: 04_task.md
**Status**: APPROVED WITH OBSERVATIONS

## Summary

Task 4.0 wires three foundational pieces: IEndpoint auto-registration, FluentValidation validator discovery, and a three-handler IExceptionHandler chain that emits Result-envelope responses. All seven new files are present and structurally correct. `Program.cs` was correctly slimmed to extension-method calls. The build passes. No controllers were touched. No `AddProblemDetails` call exists anywhere in the codebase source.

The implementation satisfies every mandatory requirement in the task file. Two minor observations are noted below; neither blocks approval.

## Reviewed Files

| File | Status | Issues |
|------|--------|--------|
| `Extensions/EndpointExtensions.cs` | OK | 0 |
| `Extensions/ValidationExtensions.cs` | OK | 0 |
| `Extensions/ExceptionHandlingExtensions.cs` | OK | 0 |
| `Infrastructure/ExceptionHandlers/InfrastructureErrors.cs` | OK | 0 |
| `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs` | OK | 0 |
| `Infrastructure/ExceptionHandlers/ConcurrencyExceptionHandler.cs` | OK | 0 |
| `Infrastructure/ExceptionHandlers/GlobalExceptionHandler.cs` | OK | 0 |
| `Program.cs` | OK | 0 |

## Issues Found

### Critical Issues

No critical issues found.

### Major Issues

No major issues found.

### Minor Issues

**Minor 1 — `MapEndpoints` return type prevents fluent chaining from `Program.cs`**

File: `Extensions/EndpointExtensions.cs`, line 21.

`MapEndpoints` returns `IApplicationBuilder` rather than `WebApplication`, which matches the minimal-api skill template exactly. The consequence is that `app.MapEndpoints()` cannot be chained with `app.MapControllers()` in a fluent call because the compiler would see `IApplicationBuilder` on the left side of `.MapControllers()`. In the current `Program.cs` the two calls are on separate lines so there is no compilation problem. The current state is safe and intentional.

If a future developer attempts to write `app.MapEndpoints().MapControllers()` it will fail to compile with a confusing error. Consider returning `WebApplication` to keep the extension consistent with other `Use*`/`Map*` extensions on `app`:

```csharp
public static WebApplication MapEndpoints(
    this WebApplication app,
    RouteGroupBuilder? routeGroupBuilder = null)
{
    ...
    return app;
}
```

This is a non-blocking stylistic improvement.

**Minor 2 — 4xx exception handlers do not log at Warning level**

Files: `Infrastructure/ExceptionHandlers/UniqueConstraintExceptionHandler.cs`, `Infrastructure/ExceptionHandlers/ConcurrencyExceptionHandler.cs`.

The techspec's Monitoring section states: "`IExceptionHandler` implementations log via `ILogger<T>` at `Error` for 5xx and at `Warning` for known 4xx DB errors (unique violation, concurrency) before writing the `Result.Failure` envelope response." The task file's requirements section does not repeat this requirement for the 4xx handlers (it only requires `Error`-level logging in `GlobalExceptionHandler`), so the task requirement as written is met. The techspec guidance is not enforced. This is worth noting as an incremental improvement: adding `ILogger<UniqueConstraintExceptionHandler>` and `ILogger<ConcurrencyExceptionHandler>` with a `Warning`-level log call before the `WriteAsJsonAsync` call would bring the implementation into full alignment with the techspec's observability guidance.

## Positive Highlights

- **Strict requirement adherence**: `AddProblemDetails` is absent from all source files. Verified by full-codebase grep. The Result-envelope-only contract is clean.
- **Exception handler chain order**: `UniqueConstraint → Concurrency → Global` registration order in `ExceptionHandlingExtensions.cs` exactly matches the task requirement. This order is important because `DbUpdateConcurrencyException` is a subclass of `DbUpdateException`; placing the concurrency handler second ensures it intercepts its specific type before the unique-constraint handler would otherwise miss it.
- **Named constants for SQL error numbers**: `SqlViolationOfUniqueIndex = 2601` and `SqlViolationOfUniqueConstraint = 2627` are declared as named constants rather than inline magic numbers, matching the code standards.
- **`internal sealed` on all handlers**: all three handlers are `internal sealed`, which prevents accidental inheritance and limits visibility to the assembly — correct for infrastructure types that implement a framework interface.
- **Early-return guard clauses**: all three `TryHandleAsync` implementations use early returns to reject non-matching exceptions, keeping nesting at one level.
- **`Program.cs` is a clean composition root**: after modification it contains exactly one `using`, the builder configuration sequence, middleware pipeline, and `app.Run()` — fully compliant with PRD requirement 6.3.
- **`UseGlobalExceptionHandling` placed before `UseDeveloperExceptionPage`**: the middleware ordering satisfies task requirement #9. The trade-off (dev exception page is bypassed in development) is intentional and documented in the task.
- **`MapEndpoints` called before `MapControllers`**: satisfies task requirement #10.
- **`WriteAsJsonAsync` uses framework-resolved JSON options**: no explicit `JsonSerializerOptions` argument is passed to `WriteAsJsonAsync`. The method resolves `IOptions<Microsoft.AspNetCore.Http.Json.JsonOptions>` from DI at runtime, which is the same options instance used by `Results.Ok(...)` / `Results.BadRequest(...)` in minimal API endpoints. The envelope shape on the wire is consistent.

## Standards Compliance

| Standard | Status |
|----------|--------|
| Code Standards | OK |
| .NET / ASP.NET Core | OK |
| REST/HTTP | OK |
| Logging | Warning — 4xx handlers silent (see Minor 2) |
| Tests | N/A per PRD |

## Recommendations

1. (Minor 2 — observability) Add `ILogger<T>` injection to `UniqueConstraintExceptionHandler` and `ConcurrencyExceptionHandler` and log at `Warning` before calling `WriteAsJsonAsync`. This aligns the implementation with the techspec monitoring guidance and will be valuable when diagnosing duplicate-insert problems in production.
2. (Minor 1 — API consistency) Change `MapEndpoints` return type from `IApplicationBuilder` to `WebApplication` so the extension is chainable from `Program.cs` like all other `app.*` calls.

## Verdict

The implementation is correct and complete against all mandatory task requirements. The build passes, the middleware pipeline order is right, the Result envelope is used consistently, `AddProblemDetails` is absent, no controllers were touched, and code standards are respected throughout. The two observations are improvements that can be applied at any time without urgency.

**The task is APPROVED WITH OBSERVATIONS. No changes are required before proceeding to task 5.0.**
