# Task 2.0: Relocate Cross-Cutting Code

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Move `Models/` → `Domain/Entities/`, move `Settings.cs` + `Static/*` + `TokenService`/`CryptographerService` → `SharedKernel/`, and convert the two cross-cutting services from `static class` to DI-registered scoped services. Keep all validation data annotations on entities **for now** — they are removed in task 13.0, once every feature has migrated to FluentValidation. This task changes namespaces and DI registration but must not change any runtime behavior.

<skills>
### Compliance with Standard Skills

- No skill directly applies. Follow the techspec "System Architecture → Component Overview" → "Modified components" and "Technical Considerations → Main Decisions #7" (instance services with DI).
</skills>

<requirements>
- Move every file under `Models/` to `Domain/Entities/` and update namespace to `BikeClub.Domain.Entities`.
- Move `Settings.cs` to `SharedKernel/Settings.cs` (namespace `BikeClub.SharedKernel`). Keep it as a static holder — `LoadSettings` continues to populate it from configuration.
- Move `Static/RoleStatic.cs` and `Static/GenderStatic.cs` to `SharedKernel/Static/` (namespace `BikeClub.SharedKernel.Static`).
- Move `Services/TokenService.cs` to `SharedKernel/Services/TokenService.cs` as `ITokenService` + `TokenService : ITokenService`. Signature: `string GenerateToken(User user)`.
- Move `Services/CryptographerSerivce.cs` to `SharedKernel/Services/CryptographerService.cs` (fix filename + class name typo) as `ICryptographerService` + `CryptographerService : ICryptographerService`. Signatures: `string Hash(string value)`, `bool CompareHash(string value, string hash)`.
- Update `Program.cs` to register both services as scoped: `services.AddScoped<ITokenService, TokenService>()`, `services.AddScoped<ICryptographerService, CryptographerService>()`.
- Update every **controller** that uses `TokenService.GenerateToken(...)` or `CryptographerService.Hash(...)` to inject the interface via constructor instead of calling the static method. (Controllers are still in place during migration; they must work.)
- Update `Data/DataContext.cs` and every `Data/Configurations/*.cs` + `Data/Seed/*.cs` + `Data/Extensions/ModelBuilderExtensions.cs` to reference the new `BikeClub.Domain.Entities` namespace.
- Do **not** remove validation data annotations from entities in this task.
- Do **not** delete the old `Services/`, `Static/`, `Models/`, or root `Settings.cs` files until their new counterparts compile and run — then delete the originals.
</requirements>

## Subtasks

- [x] 2.1 Move `Models/*.cs` → `Domain/Entities/` and update each file's namespace. Update all `using BikeClub.Models;` references across the codebase (controllers, Data/, Services/).
- [x] 2.2 Move `Settings.cs` → `SharedKernel/Settings.cs`; update all `using BikeClub;` references that resolve to `Settings`.
- [x] 2.3 Move `Static/*` → `SharedKernel/Static/`; update `using BikeClub.Static;` references.
- [x] 2.4 Introduce `ITokenService` + `TokenService` under `SharedKernel/Services/`; remove the static class; update `AccountController` and `UserController` (and any other consumer) to inject `ITokenService`.
- [x] 2.5 Introduce `ICryptographerService` + `CryptographerService` under `SharedKernel/Services/` (fix filename typo); update `AccountController` and `UserController` to inject `ICryptographerService`.
- [x] 2.6 Register both services as scoped in `Program.cs` (temporary — task 3.0 will move this into `ConfigureServices` extension).
- [x] 2.7 Update `Data/DataContext.cs`, `Data/Configurations/*`, `Data/Seed/*`, `Data/Extensions/ModelBuilderExtensions.cs` to new namespaces.
- [x] 2.8 Delete now-empty `Models/`, `Services/` (except `ExceptionHandlerService.cs` — removed in task 13.0), `Static/`, and root `Settings.cs`.
- [x] 2.9 Manual Verification.

## Implementation Details

See `techspec.md` → "System Architecture → Component Overview" (Modified components) and "Technical Considerations → Main Decisions #7" for the rationale behind converting the services to scoped DI. The `minimal-api` skill does not apply here; no new endpoints are introduced.

## Success Criteria

- `dotnet build` succeeds.
- `Models/`, root `Settings.cs`, and `Static/` folders no longer exist.
- `Services/ExceptionHandlerService.cs` still exists (removed in task 13.0); the other two service files are gone.
- `Domain/Entities/` contains all 11 entity files.
- Every controller that previously called `TokenService.GenerateToken` or `CryptographerService.Hash` now receives the services via constructor injection.
- `POST /v1/accounts/login` and `POST /v1/accounts/register` continue to work identically (same success payloads, same JWT claim set, same password-hash compatibility against seeded data).

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [x] **Manual Verification**
  - [x] `dotnet build` completes with zero errors.
  - [x] `POST /v1/accounts/login` with a seeded user returns the same `{ user, token, expiresIn }` body and a valid JWT.
  - [x] `POST /v1/accounts/register` with a new email succeeds and returns a token.
  - [x] `GET /v1/users` (as Monitor) returns the user list unchanged.
  - [x] Decode the issued JWT and confirm the `NameIdentifier`, `Name`, and `Role` claims match pre-refactor output.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Domain/Entities/*.cs` (moved from `Models/*.cs`)
- `SharedKernel/Settings.cs` (moved from root)
- `SharedKernel/Static/RoleStatic.cs`, `SharedKernel/Static/GenderStatic.cs` (moved)
- `SharedKernel/Services/ITokenService.cs`, `SharedKernel/Services/TokenService.cs` (new interface + instance class)
- `SharedKernel/Services/ICryptographerService.cs`, `SharedKernel/Services/CryptographerService.cs` (new interface + instance class; filename typo fixed)
- `Program.cs` (temporary DI registration for the two services)
- `Data/DataContext.cs`, `Data/Configurations/*.cs`, `Data/Seed/*.cs`, `Data/Extensions/ModelBuilderExtensions.cs` (namespace updates)
- All controllers that consume `ITokenService` / `ICryptographerService` (injection updates)
