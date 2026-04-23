# Task 5.0: Account Feature Slice (Login, Register)

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `AccountController` to `Features/Account/`. This is the first feature slice and exercises the full pattern end-to-end: FluentValidation via DI, `ITokenService` + `ICryptographerService` via DI, `DataContext` via DI, Result pattern for validation and business errors (invalid credentials → 404, email already exists → 409), and RFC 7807 error envelope. After this task, `AccountController.cs` is deleted.

<skills>
### Compliance with Standard Skills

- **`result-pattern`** — handlers return `Result<LoginResponse>` / `Result<RegisterResponse>`; validation failures become `ValidationResult` → 400 `ValidationProblem`.
- **`minimal-api`** — each operation is an `IEndpoint` under `Features/Account/<Operation>/`.
</skills>

<requirements>
- Create `Features/Account/Login/` with `LoginRequest`, `LoginResponse`, `LoginValidator`, `LoginHandler`, `LoginEndpoint`.
  - Route: `POST v1/accounts/login`, `AllowAnonymous`.
  - `LoginRequest(string Email, string Password)`.
  - `LoginValidator`: email non-empty + valid email; password non-empty.
  - `LoginHandler`: validate request → hash password via `ICryptographerService` → look up user → compare → build `LoginResponse { user, token, expiresIn }` (same shape as today, with `user.Password = "***********"`) or `Result.Failure(AccountErrors.InvalidCredentials)` (`ErrorType.NotFound`, mirrors today's 404).
  - `LoginEndpoint`: `.ToIResult(onSuccess: r => TypedResults.Ok(r))`.
- Create `Features/Account/Register/` with `RegisterRequest`, `RegisterResponse`, `RegisterValidator`, `RegisterHandler`, `RegisterEndpoint`.
  - Route: `POST v1/accounts/register`, `AllowAnonymous`.
  - `RegisterRequest` carries the fields `Email`, `Password`, `Phone`, `Name`, `LastName`, `GenderCode` (the subset that was previously `User` with annotations).
  - `RegisterValidator`: mirror the existing data-annotation rules exactly (`Email` required + `EmailAddress` + length 3..100; `Password` required + length 6..30; `Phone` required + regex `^(?:\()[0-9]{2}(?:\))\s?[0-9]{4,5}(?:-)[0-9]{4}$` + length 6..20; `Name` required + length 1..50; `LastName` required + length 1..50). Validation failure codes must include the property name for the `ValidationProblem` output.
  - `RegisterHandler`: validate → check `Users.Count(u => u.Email == email) > 0` → if exists, `Result.Failure(AccountErrors.EmailAlreadyExists)` (`ErrorType.Conflict` → 409). Otherwise: set `RoleName = RoleStatic.Cyclist`, hash password, create empty `ShopCart`, save, build same `RegisterResponse { user, token, expiresIn }` shape as before.
- Create `Features/Account/Shared/AccountErrors.cs` with `InvalidCredentials` (NotFound), `EmailAlreadyExists` (Conflict).
- Delete `Controllers/AccountController.cs`.
- Keep success payload shapes byte-identical: `{ user: <User with masked password>, token: "<jwt>", expiresIn: "<ISO-8601>" }`.
</requirements>

## Subtasks

- [ ] 5.1 Create `Features/Account/Shared/AccountErrors.cs`.
- [ ] 5.2 Create the `Login` operation folder with all five files.
- [ ] 5.3 Create the `Register` operation folder with all five files.
- [ ] 5.4 Delete `Controllers/AccountController.cs`.
- [ ] 5.5 Manual Verification.

## Implementation Details

See `techspec.md` → "Implementation Design → Main Interfaces" (handler + endpoint examples) and "API Endpoints" (account row) for exact routes and auth. For the success-payload shape, diff against the pre-refactor controller — response record fields must serialize to the same JSON keys (casing/naming) as today.

## Success Criteria

- `Features/Account/` exists with `Login` and `Register` operation folders.
- `Controllers/AccountController.cs` no longer exists.
- `POST /v1/accounts/login` and `POST /v1/accounts/register` continue to succeed and return identical success JSON to pre-refactor output (diff a captured response from before/after).
- Error responses now use RFC 7807 ProblemDetails (this is an intentional, PRD-sanctioned change).

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Happy path: `POST /v1/accounts/register` with a fresh email + valid fields → 200, `{ user, token, expiresIn }`; JWT decodes to the same claims as before (`NameIdentifier`, `Name`, `Role: "Cyclist"`).
  - [ ] Duplicate email: second register call with same email → 409 ProblemDetails `{ type, title, status: 409, detail, code: "Account.EmailAlreadyExists" }`.
  - [ ] Login happy path: `POST /v1/accounts/login` with correct credentials → 200, same shape.
  - [ ] Login with wrong password → 404 ProblemDetails, `code: "Account.InvalidCredentials"`.
  - [ ] Login with missing email → 400 ValidationProblem with `errors.Email` entry.
  - [ ] Register with phone in wrong format → 400 ValidationProblem with `errors.Phone` entry.
  - [ ] Freshly registered user can call `GET /v1/bikes` with the returned token → 200.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Account/Shared/AccountErrors.cs` (new)
- `Features/Account/Login/*` (new — 5 files)
- `Features/Account/Register/*` (new — 5 files)
- `Controllers/AccountController.cs` (deleted)
