# Task 6.0: Address Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `AddressController` to `Features/Address/` across five operations. Simplest CRUD slice — use it as the canonical template that tasks 7.0–12.0 follow. Deletes `Controllers/AddressController.cs` on completion.

<skills>
### Compliance with Standard Skills

- **`result-pattern`** — `Result<T>` for queries/creates/updates; `Result` (no payload) for delete.
- **`minimal-api`** — one `IEndpoint` per operation.
</skills>

<requirements>
- Create `Features/Address/` with operation folders: `GetAddress`, `GetAddressById`, `CreateAddress`, `UpdateAddress`, `DeleteAddress`.
- Routes and auth (preserve exactly):
  - `GET v1/addresses` — `RequireAuthorization()`.
  - `GET v1/addresses/{id:int}` — `RequireAuthorization()`. **Behavior improvement:** today returns 200 with `null` body when missing; post-migration must return 404 ProblemDetails via `Result.Failure(AddressErrors.NotFound)` (`ErrorType.NotFound`). Documented in techspec "Known Risks".
  - `POST v1/addresses` — `RequireAuthorization()`.
  - `PUT v1/addresses/{id:int}` — `RequireAuthorization()`. Preserve the "cannot change Id" rule as a handler-level check → `Result.Failure(AddressErrors.IdMismatch)` (`ErrorType.Validation` → 400).
  - `DELETE v1/addresses/{id:int}` — `RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })`. Missing record → 404 (matches today).
- `CreateAddressValidator` and `UpdateAddressValidator`: mirror the existing data-annotation rules on the `Address` entity (Street 3..50, Complement 1..50, State 2..2, City 1..30, ZipCode required/non-zero). Since Create/Update share identical rules, centralize in `Features/Address/Shared/AddressRequestValidator.cs` and reuse from both.
- `Features/Address/Shared/AddressErrors.cs`: `NotFound`, `IdMismatch`.
- Delete `Controllers/AddressController.cs`.
</requirements>

## Subtasks

- [ ] 6.1 Create `Features/Address/Shared/` with `AddressErrors.cs` and `AddressRequestValidator.cs` (base validator reused by Create/Update).
- [ ] 6.2 Create `GetAddress` operation (no request body, no validator).
- [ ] 6.3 Create `GetAddressById` operation (route parameter; no body; returns 404 when missing).
- [ ] 6.4 Create `CreateAddress` operation (uses shared validator).
- [ ] 6.5 Create `UpdateAddress` operation (uses shared validator + IdMismatch check).
- [ ] 6.6 Create `DeleteAddress` operation (Monitor-only; 404 on missing).
- [ ] 6.7 Delete `Controllers/AddressController.cs`.
- [ ] 6.8 Manual Verification.

## Implementation Details

See `techspec.md` → "Implementation Design → Main Interfaces" for the handler+endpoint template, and "API Endpoints" (address row) for the exact route and role table.

Default-case `Result.Failure` with `ErrorType.Failure` is intentionally NOT used here — every expected failure is categorized so the HTTP adapter picks the right status code. Generic framework exceptions (DB uniqueness, concurrency) are handled by the `IExceptionHandler` chain from task 4.0.

## Success Criteria

- All five routes behave identically to before on the happy path (same success payloads).
- `GET v1/addresses/{id}` with an invalid id now returns 404 ProblemDetails (intentional improvement).
- `Controllers/AddressController.cs` is gone.
- Auto-registration picks up the five new endpoints without any change to `Program.cs`.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Cyclist token: `GET /v1/addresses` → 200, list. `POST /v1/addresses` with valid body → 200, created Address.
  - [ ] Cyclist token: `DELETE /v1/addresses/{id}` → 403 (Monitor role required).
  - [ ] Monitor token: `DELETE /v1/addresses/{id}` → 200 message, then `GET /v1/addresses/{id}` → 404 ProblemDetails.
  - [ ] `POST /v1/addresses` with empty `City` → 400 ValidationProblem with `errors.City`.
  - [ ] `PUT /v1/addresses/99` with body `{ "id": 100, … }` → 400 ProblemDetails, `code: "Address.IdMismatch"`.
  - [ ] Unauthenticated: `GET /v1/addresses` → 401.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Address/Shared/AddressErrors.cs` (new)
- `Features/Address/Shared/AddressRequestValidator.cs` (new)
- `Features/Address/GetAddress/*` (new)
- `Features/Address/GetAddressById/*` (new)
- `Features/Address/CreateAddress/*` (new)
- `Features/Address/UpdateAddress/*` (new)
- `Features/Address/DeleteAddress/*` (new)
- `Controllers/AddressController.cs` (deleted)
