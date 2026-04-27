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

See `techspec.md` → "API Endpoints" (tour row). Success values preserve today's shape (Tour entity serialized as-is, matching EF reference-handling `IgnoreCycles` behavior). Those values now live at `response.value` of the `Result<T>` envelope; clients reading `response.value` see exactly today's bare body. DELETE returns `204 NoContent` on success.

## Success Criteria

- All five routes preserve route, verb, success status code. Success body is the `Result<T>` envelope (or `204 NoContent` for DELETE); failures are the Result envelope at the appropriate status.
- `Controllers/TourController.cs` gone.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Cyclist: `GET /v1/tours` → 200 with Result envelope (`response.value` = list); `POST /v1/tours` → 403 (no body).
  - [ ] Monitor: `POST /v1/tours` valid body → 200 with `Result<TourResponse>` envelope; `POST` with invalid body → 400 with validation Result envelope (`errors[]` populated).
  - [ ] Monitor: `PUT /v1/tours/1` with `{ "id": 2, ... }` → 400 with `error.code: "Tour.IdMismatch"`.
  - [ ] Monitor: `DELETE /v1/tours/{id}` hit → 204 NoContent; miss → 404 with `Result` envelope, `error.code: "Tour.NotFound"`.
  - [ ] `GET /v1/tours/9999` → 404 Result envelope (was 200 `null` before — documented behavior improvement).

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Tour/*` (new)
- `Controllers/TourController.cs` (deleted)
