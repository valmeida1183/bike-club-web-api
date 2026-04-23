# Task 9.0: Tour Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `TourController`. Straightforward CRUD — same pattern as Address but with Monitor-only writes and deletes. No navigation-property includes on GET.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**, **`minimal-api`**.
</skills>

<requirements>
- `Features/Tour/GetTour/` — `GET v1/tours`, `RequireAuthorization()`.
- `Features/Tour/GetTourById/` — `GET v1/tours/{id:int}`, `RequireAuthorization()`. 404 on missing.
- `Features/Tour/CreateTour/` — `POST v1/tours`, Monitor.
- `Features/Tour/UpdateTour/` — `PUT v1/tours/{id:int}`, Monitor. Preserve "cannot change Id" → `TourErrors.IdMismatch`.
- `Features/Tour/DeleteTour/` — `DELETE v1/tours/{id:int}`, Monitor. 404 on missing.
- Validators mirror `Tour` entity data annotations.
- Shared `TourErrors.cs` with `NotFound`, `IdMismatch`.
- Delete `Controllers/TourController.cs`.
</requirements>

## Subtasks

- [ ] 9.1 Shared errors + request validator.
- [ ] 9.2 `GetTour`.
- [ ] 9.3 `GetTourById`.
- [ ] 9.4 `CreateTour`.
- [ ] 9.5 `UpdateTour` (IdMismatch).
- [ ] 9.6 `DeleteTour`.
- [ ] 9.7 Delete `Controllers/TourController.cs`.
- [ ] 9.8 Manual Verification.

## Implementation Details

See `techspec.md` → "API Endpoints" (tour row). Response payload for success preserves today's shape (Tour entity serialized as-is, matching EF reference-handling `IgnoreCycles` behavior configured in `Program.cs`).

## Success Criteria

- All five routes behave identically on the happy path.
- `Controllers/TourController.cs` gone.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Cyclist: `GET /v1/tours` → 200; `POST /v1/tours` → 403.
  - [ ] Monitor: `POST /v1/tours` valid body → 200; `POST` with invalid body → 400 ValidationProblem.
  - [ ] Monitor: `PUT /v1/tours/1` with `{ "id": 2, ... }` → 400 `Tour.IdMismatch`.
  - [ ] Monitor: `DELETE /v1/tours/{id}` hit → 200; miss → 404 ProblemDetails.
  - [ ] `GET /v1/tours/9999` → 404 ProblemDetails (was 200 `null` before — documented behavior improvement).

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Tour/*` (new)
- `Controllers/TourController.cs` (deleted)
