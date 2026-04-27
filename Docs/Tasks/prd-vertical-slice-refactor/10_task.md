# Task 10.0: User Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `UserController`. Today's controller exposes four operations: `Get`, `GetById`, `PostMonitor` (Monitor-only creation that forces `RoleName = Monitor` and hashes the password), and `Put` (Monitor-only update). No delete exists — do not add one.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**, **`minimal-api`**.
</skills>

<requirements>
- `Features/User/GetUser/` — `GET v1/users`, `RequireAuthorization()`. Masked password (`"***********"`) preserved? Verify against today's code — today's controller returns users as-is (no masking on list). **Match that behavior exactly.**
- `Features/User/GetUserById/` — `GET v1/users/{id:int}`, `RequireAuthorization()`. 404 on missing.
- `Features/User/CreateUserAsMonitor/` — `POST v1/users`, Monitor. Handler forces `RoleName = RoleStatic.Monitor`, hashes password via `ICryptographerService`. Validator mirrors today's `User` entity annotations (email/password/phone/name/lastName rules — see task 5.0 Register for the exact rule set).
- `Features/User/UpdateUser/` — `PUT v1/users/{id:int}`, Monitor. Preserve "cannot change Id" → `UserErrors.IdMismatch`.
- Shared `UserErrors.cs` with `NotFound`, `IdMismatch`.
- Delete `Controllers/UserController.cs`.
</requirements>

## Subtasks

- [ ] 10.1 Shared errors + validator (can reuse Register's validator if rules are identical; prefer composition via a `UserPersonalInfoValidator` under `Features/User/Shared/`).
- [ ] 10.2 `GetUser`.
- [ ] 10.3 `GetUserById`.
- [ ] 10.4 `CreateUserAsMonitor` (forces role; hashes password).
- [ ] 10.5 `UpdateUser` (IdMismatch).
- [ ] 10.6 Delete `Controllers/UserController.cs`.
- [ ] 10.7 Manual Verification.

## Implementation Details

See `techspec.md` → "API Endpoints" (user row). Be careful with the `Put` behavior: today's controller calls `context.Entry<User>(model).State = EntityState.Modified`, which **does not hash the password** if the caller sends one — reproduce that exactly unless you intend to change the behavior (PRD says do not change business logic; preserve it).

## Success Criteria

- All four routes preserve route, verb, success status code. Success bodies are the `Result<T>` envelope (the previously bare value is at `response.value`); failures are the Result envelope at the appropriate status.
- `Controllers/UserController.cs` gone.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Cyclist token: `GET /v1/users` → 200 Result envelope (`response.value` = list). `POST /v1/users` → 403 (no body).
  - [ ] Monitor: `POST /v1/users` valid body → 200 with `Result<UserResponse>` envelope; inspect DB — `RoleName` is `"Monitor"` and `Password` is hashed (not the raw string sent).
  - [ ] Monitor: `POST /v1/users` with invalid email → 400 with validation Result envelope (`errors[]` includes a `code: "Email"` entry).
  - [ ] Monitor: `PUT /v1/users/1` with body where `id = 2` → 400 with `error.code: "User.IdMismatch"`, `error.type: "Validation"`.
  - [ ] `GET /v1/users/9999` → 404 Result envelope, `error.code: "User.NotFound"` (was 200 null before).

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/User/*` (new)
- `Controllers/UserController.cs` (deleted)
