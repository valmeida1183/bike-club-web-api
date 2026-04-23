# Task 1.0: SharedKernel & Result Pattern Foundation

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Create the foundational folders (`SharedKernel/`, `Domain/`, `Extensions/`, `Infrastructure/ExceptionHandlers/`) and land the Result pattern core plus the `IEndpoint` abstraction. This task introduces no new behavior — the existing controllers, routes, and payloads keep working exactly as before. After this task, the types needed to build feature slices exist in the solution but are not yet consumed by any feature.

<skills>
### Compliance with Standard Skills

- **`result-pattern`** — governs the shapes of `Result`, `Result<T>`, `Error`, and `ValidationResult`. Follow the skill verbatim with one addition documented in the techspec: extend `Error` with an `ErrorType` enum field so the HTTP adapter can select a status code without string-matching `Code`.
- **`minimal-api`** — governs the `IEndpoint` interface signature (`void MapEndpoint(IEndpointRouteBuilder app)`).
</skills>

<requirements>
- Create `SharedKernel/`, `Domain/Entities/`, `Extensions/`, `Infrastructure/ExceptionHandlers/` folders at project root.
- Add `SharedKernel/IEndpoint.cs` with the exact signature from the `minimal-api` skill.
- Add `SharedKernel/Results/` containing `Error.cs` (with `ErrorType` enum), `Result.cs` (`Result` and `Result<T>`), `ValidationResult.cs`, and `ResultExtensions.cs` following the `result-pattern` skill.
- Add `SharedKernel/Http/ResultExtensions.cs` with `ToIResult()` / `ToIResult<T>(Func<T, IResult>? onSuccess = null)` that map `ErrorType` → `TypedResults.Problem` / `ValidationProblem` / `Ok` per the techspec "Integration Points → Error contract" section.
- Add `FluentValidation` and `FluentValidation.DependencyInjectionExtensions` NuGet packages to `bike-club-api.csproj`. Do **not** wire validators up yet — that is task 4.0.
- The project MUST still build (`dotnet build`) and run (`dotnet watch run`) with zero behavior change.
</requirements>

## Subtasks

- [ ] 1.1 Create the four new folders and a placeholder `.gitkeep` only where needed to keep them tracked.
- [ ] 1.2 Add `IEndpoint.cs` under `SharedKernel/` (no consumers yet).
- [ ] 1.3 Add `Error.cs` with the `ErrorType` enum (`Failure`, `Validation`, `NotFound`, `Conflict`, `Unauthorized`, `Forbidden`) and `Error.None` / `Error.NullValue` sentinels.
- [ ] 1.4 Add `Result.cs` (base `Result` with `IsSuccess` / `IsFailure` / `Error` + factories, and `Result<T>` with `Value` accessor and implicit conversions).
- [ ] 1.5 Add `ValidationResult.cs` (and `ValidationResult<T>`) aggregating multiple `Error[]`.
- [ ] 1.6 Add `ResultExtensions.cs` (functional `Map`, `Bind`, `Tap`, `Match`, `Ensure`, `Combine` — see the `result-pattern` skill).
- [ ] 1.7 Add `SharedKernel/Http/ResultExtensions.cs` with `ToIResult()` overloads that map `ErrorType` to `TypedResults`.
- [ ] 1.8 Add `FluentValidation` + `FluentValidation.DependencyInjectionExtensions` packages.
- [ ] 1.9 Manual Verification.

## Implementation Details

See `techspec.md` → "System Architecture → Component Overview" and "Implementation Design → Main Interfaces" for the exact shapes.

Do **not** move `Models/`, `Services/`, `Static/`, or `Settings.cs` yet — that is task 2.0. Do **not** modify `Program.cs` — that is task 3.0. Do **not** register `IEndpoint` or validators yet — that is task 4.0.

## Success Criteria

- `dotnet build` succeeds with zero warnings from the new files.
- `dotnet watch run` serves all existing `/v1/` routes identically to before (success responses byte-identical; error responses unchanged because no handler is calling `Result` yet).
- `Find in Files` for `namespace BikeClub.SharedKernel` returns the new Result/Error/Endpoint types.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] `dotnet build` completes with no errors.
  - [ ] `dotnet watch run` starts; Swagger UI at `https://localhost:5001/swagger` still lists all existing endpoints.
  - [ ] Smoke: `GET /v1/genders` (anon), `POST /v1/accounts/login` (with a seeded user) still return the same responses as before.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `SharedKernel/IEndpoint.cs` (new)
- `SharedKernel/Results/Error.cs` (new)
- `SharedKernel/Results/Result.cs` (new)
- `SharedKernel/Results/ValidationResult.cs` (new)
- `SharedKernel/Results/ResultExtensions.cs` (new)
- `SharedKernel/Http/ResultExtensions.cs` (new)
- `bike-club-api.csproj` (modified — new package references)
