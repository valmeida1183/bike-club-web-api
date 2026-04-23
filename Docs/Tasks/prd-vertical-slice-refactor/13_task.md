# Task 13.0: Cleanup & Documentation

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Final task. Delete the now-empty `Controllers/` folder, retire the legacy `ExceptionHandlerService`, strip all `System.ComponentModel.DataAnnotations` **validation** attributes from entities (FluentValidation has fully replaced them), remove the `app.MapControllers()` call from `Program.cs`, and update `CLAUDE.md` to reflect the new architecture.

<skills>
### Compliance with Standard Skills

- No skill applies directly. This is housekeeping that makes the success criteria in the PRD ("0 files under `Controllers/`", "0 DataAnnotations validation attributes") measurably true.
</skills>

<requirements>
- Confirm `Controllers/` is empty (all 11 controllers already deleted in tasks 5.0–12.0). Delete the folder.
- Delete `Services/ExceptionHandlerService.cs`. If `Services/` folder is empty after this delete, delete the folder.
- Remove `app.MapControllers()` and the `AddControllers`-related DI registration from `Program.cs` (and from the extension class that holds it).
- Remove the JSON options `ReferenceHandler.IgnoreCycles` setting from the controllers DI block — migrate it to Minimal API's JSON options: `builder.Services.ConfigureHttpJsonOptions(options => options.SerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles);` via a new `Extensions/JsonExtensions.cs`.
- For every file under `Domain/Entities/`, remove all `using System.ComponentModel.DataAnnotations;` usages and every `[Required]`, `[StringLength]`, `[MaxLength]`, `[MinLength]`, `[EmailAddress]`, `[RegularExpression]`, `[Display]`, `[Compare]`, `[Range]`, `[DataType]`, and any other `System.ComponentModel.DataAnnotations` **validation** attribute. Do **NOT** remove mapping attributes (`[Key]`, `[ForeignKey]`, `[Column]`, `[Table]`, `[DatabaseGenerated]`) even though these entities currently do not use them — the `Data/Configurations/*` files own mapping via Fluent API.
- Update `CLAUDE.md`:
  - Architecture section: replace the "Layers" and "Project Structure" blocks with the new folder layout (`Features/`, `Extensions/`, `SharedKernel/`, `Domain/`, `Infrastructure/ExceptionHandlers/`, `Data/`).
  - Document the Result pattern briefly (point to the `result-pattern` skill).
  - Document FluentValidation usage (one validator per request record, invoked by the handler).
  - Document endpoint auto-registration (any `IEndpoint` is mapped automatically — no `Program.cs` edit needed).
  - Remove all references to `Controllers/`, root-level `Services/`, root-level `Static/`, root-level `Settings.cs`, and `Models/`.
  - Add a brief note about the error response contract (RFC 7807 ProblemDetails).
- Final end-to-end smoke test.
</requirements>

## Subtasks

- [ ] 13.1 Delete `Controllers/` folder.
- [ ] 13.2 Delete `Services/ExceptionHandlerService.cs` (and the `Services/` folder if empty).
- [ ] 13.3 Remove `AddControllers` and `MapControllers` from `Program.cs` / extensions; replace the `ReferenceHandler.IgnoreCycles` JSON option with `ConfigureHttpJsonOptions` in a new `Extensions/JsonExtensions.cs`.
- [ ] 13.4 Strip validation `DataAnnotations` from every file in `Domain/Entities/`. Remove now-unused `using System.ComponentModel.DataAnnotations;`.
- [ ] 13.5 Update `CLAUDE.md` per the requirements above.
- [ ] 13.6 Full-surface Manual Verification — every `/v1/` endpoint.

## Implementation Details

See `techspec.md` → "Technical Considerations → Relevant and Dependent Files" for the canonical list of files affected here.

Validation assertion: after this task, `grep -r "System.ComponentModel.DataAnnotations" Domain/` MUST return zero matches. Similarly, `ls Controllers/` MUST report "No such file or directory".

## Success Criteria (PRD measurable outcomes)

- 0 files under `Controllers/`.
- 0 `System.ComponentModel.DataAnnotations` validation attributes in `Domain/Entities/*`.
- 100% of endpoints served by `Features/<Feature>/<Operation>/*Endpoint.cs` classes registered via `MapEndpoints()`.
- `Program.cs` contains only extension calls + middleware + `app.Run()` — no inline service or middleware configuration.
- All existing `/v1/` routes respond with identical success status codes and payloads.
- `CLAUDE.md` no longer references `Controllers/`, `Models/`, root `Services/`, `Static/`, or root `Settings.cs`.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification — full regression sweep** (Swagger/Postman collection):
  - [ ] `POST /v1/accounts/login` and `/register` (happy + validation + conflict).
  - [ ] `GET/POST/PUT/DELETE` across Address, Bike, Category, Difficulty, Gender, Role, Tour, User — happy + Monitor-required + validation + IdMismatch.
  - [ ] `POST /v1/purchases` (upsert) + `DELETE /v1/purchases/{id}`.
  - [ ] Full ShopCart flow: `GET/POST/PATCH address/add-purchase/update-purchase/remove-purchase`.
  - [ ] Anonymous `GET /v1/genders`, `GET /v1/categories`, `GET /v1/difficulties` — confirm `Cache-Control` / `Vary: User-Agent` still present.
  - [ ] `GET /Resources/Images/<seeded image>` returns the file.
  - [ ] Swagger UI at `https://localhost:5001/swagger` renders, lists every route, and the schemas for ProblemDetails responses appear for error responses.
  - [ ] `dotnet build` → 0 errors, 0 warnings related to the refactor.
  - [ ] Grep repo for `BikeClub.Controllers`, `BikeClub.Models`, `BikeClub.Static`, `BikeClub.Services` — zero matches.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Controllers/` (deleted)
- `Services/ExceptionHandlerService.cs` (deleted); `Services/` folder removed if empty
- `Program.cs` (remove `AddControllers`/`MapControllers`)
- `Extensions/JsonExtensions.cs` (new — `ConfigureHttpJsonOptions`)
- `Domain/Entities/*.cs` (strip validation annotations)
- `CLAUDE.md` (rewritten architecture section)
