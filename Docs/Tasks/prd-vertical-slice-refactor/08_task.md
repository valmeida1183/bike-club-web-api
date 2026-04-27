# Task 8.0: Bike Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `BikeController`. The interesting operation is `GetBike`, which takes six optional query-string filters (`categoryId`, `genderCode`, `price`, `gears`, `frameSize`, `rimSize`) and eagerly loads `Category` and `Gender` navigation properties. Preserve filter semantics exactly: each parameter, when provided, applies a `<=` or equality predicate.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**.
- **`minimal-api`** — `GetBikeRequest` is bound via `[AsParameters]` or individual query parameters.
</skills>

<requirements>
- `Features/Bike/GetBike/` — `GET v1/bikes`, `RequireAuthorization()`. `GetBikeRequest` carries the six optional filters; handler reproduces the existing LINQ predicate verbatim.
- `Features/Bike/GetBikeById/` — `GET v1/bikes/{id:int}`, `RequireAuthorization()`. Eager-loads `Category`, `Gender`. 404 on missing.
- `Features/Bike/CreateBike/` — `POST v1/bikes`, Monitor.
- `Features/Bike/UpdateBike/` — `PUT v1/bikes/{id:int}`, Monitor. Preserve "cannot change Id" → `BikeErrors.IdMismatch`.
- `Features/Bike/DeleteBike/` — `DELETE v1/bikes/{id:int}`, Monitor. 404 on missing.
- Validator rules mirror the current `Bike` entity data annotations (inspect `Models/Bike.cs` at the time of task 2.0 — the annotations are still present until task 13.0).
- Delete `Controllers/BikeController.cs`.
</requirements>

## Subtasks

- [ ] 8.1 Shared: `BikeErrors.cs`, `BikeRequestValidator.cs` (if Create/Update rules overlap).
- [ ] 8.2 `GetBike` with filters (bind via `[AsParameters]` on `GetBikeRequest`).
- [ ] 8.3 `GetBikeById` with `Include(b => b.Category).Include(b => b.Gender)`.
- [ ] 8.4 `CreateBike`.
- [ ] 8.5 `UpdateBike` (IdMismatch check).
- [ ] 8.6 `DeleteBike`.
- [ ] 8.7 Delete `Controllers/BikeController.cs`.
- [ ] 8.8 Manual Verification.

## Implementation Details

See `techspec.md` → "API Endpoints" (bike row). The `GetBike` LINQ:
```csharp
.Where(b => (!categoryId.HasValue || b.CategoryId == categoryId) &&
            (string.IsNullOrEmpty(genderCode) || b.GenderCode == genderCode) &&
            (!price.HasValue || b.Price <= price) &&
            (!gears.HasValue || b.Gears <= gears) &&
            (!frameSize.HasValue || b.FrameSize <= frameSize) &&
            (!rimSize.HasValue || b.RimSize <= rimSize))
```
must be reproduced byte-for-byte (the `<=` semantics are intentional).

## Success Criteria

- All five routes preserve route, verb, success status code. Success **bodies** are now the `Result<T>` envelope (the previously bare value lives at `response.value`); DELETE success is `204 NoContent` (no body).
- Query-string filters return the same filtered lists at `response.value` as before (verify with a couple of real filter combinations).
- `Controllers/BikeController.cs` gone.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] `GET /v1/bikes` → 200 with `Result<…>` envelope; `response.value` is the full list with nested `Category` and `Gender`.
  - [ ] `GET /v1/bikes?price=2000&gears=21` → `response.value` matches today's filtered subset for the same query.
  - [ ] `GET /v1/bikes/{id}` hit → 200 Result envelope with nested includes at `response.value`; miss → 404 with `Result` envelope, `error.code: "Bike.NotFound"`.
  - [ ] Cyclist: `POST /v1/bikes` → 403 (no body); Monitor: → 200 Result envelope.
  - [ ] Monitor: `PUT /v1/bikes/1` with `{ "id": 2, ... }` → 400 with `error.code: "Bike.IdMismatch"`.
  - [ ] Monitor: `DELETE /v1/bikes/9999` → 404 Result envelope; `DELETE /v1/bikes/{existingId}` → 204 NoContent (no body).

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Bike/Shared/*` (new)
- `Features/Bike/GetBike/*` (new)
- `Features/Bike/GetBikeById/*` (new)
- `Features/Bike/CreateBike/*` (new)
- `Features/Bike/UpdateBike/*` (new)
- `Features/Bike/DeleteBike/*` (new)
- `Controllers/BikeController.cs` (deleted)
