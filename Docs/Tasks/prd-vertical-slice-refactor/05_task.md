# Task 5.0: Account Feature Slice (Login, Register)

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `AccountController` to `Features/Account/`. This is the first feature slice and exercises the full pattern end-to-end: FluentValidation via DI, `ITokenService` + `ICryptographerService` via DI, `DataContext` via DI, Result pattern for validation and business errors (invalid credentials → 404, email already exists → 409), and the standardized `Result`/`Result<T>` response envelope on every body-producing response. After this task, `AccountController.cs` is deleted.

<skills>
### Compliance with Standard Skills

- **`result-pattern`** — handlers return `Result<LoginResponse>` / `Result<RegisterResponse>`; validation failures become `ValidationResult<T>` and the endpoint serializes the whole Result envelope at status 400.
- **`minimal-api`** — each operation is an `IEndpoint` under `Features/Account/<Operation>/`. Endpoints serialize the Result via `.ToIResult()` — body is the Result object, never `result.Value`, never ProblemDetails.
</skills>

<requirements>
- Create `Features/Account/Login/` with `LoginRequest`, `LoginResponse`, `LoginValidator`, `LoginHandler`, `LoginEndpoint`.
  - Route: `POST v1/accounts/login`, `AllowAnonymous`.
  - `LoginRequest(string Email, string Password)`.
  - `LoginValidator`: email non-empty + valid email; password non-empty.
  - `LoginHandler`: validate request → hash password via `ICryptographerService` → look up user → compare → build `LoginResponse { user, token, expiresIn }` (same shape as today, with `user.Password = "***********"`) or `Result.Failure(AccountErrors.InvalidCredentials)` (`ErrorType.NotFound`, mirrors today's 404).
  - `LoginEndpoint`: `.ToIResult()` (default success branch returns `Results.Ok(result)`, body is the full `Result<LoginResponse>` envelope; failure → `Results.NotFound(result)` for `InvalidCredentials`).
- Create `Features/Account/Register/` with `RegisterRequest`, `RegisterResponse`, `RegisterValidator`, `RegisterHandler`, `RegisterEndpoint`.
  - Route: `POST v1/accounts/register`, `AllowAnonymous`.
  - `RegisterRequest` carries the fields `Email`, `Password`, `Phone`, `Name`, `LastName`, `GenderCode` (the subset that was previously `User` with annotations).
  - `RegisterValidator`: mirror the existing data-annotation rules exactly (`Email` required + `EmailAddress` + length 3..100; `Password` required + length 6..30; `Phone` required + regex `^(?:\()[0-9]{2}(?:\))\s?[0-9]{4,5}(?:-)[0-9]{4}$` + length 6..20; `Name` required + length 1..50; `LastName` required + length 1..50). Validation failure `Error.Code` must be the property name (e.g., `"Email"`, `"Phone"`) so clients can locate per-property errors in `result.errors[]`.
  - `RegisterHandler`: validate → check `Users.Count(u => u.Email == email) > 0` → if exists, `Result.Failure(AccountErrors.EmailAlreadyExists)` (`ErrorType.Conflict` → 409). Otherwise: set `RoleName = RoleStatic.Cyclist`, hash password, create empty `ShopCart`, save, build same `RegisterResponse { user, token, expiresIn }` shape as before.
- Create `Features/Account/Shared/AccountErrors.cs` with `InvalidCredentials` (NotFound), `EmailAlreadyExists` (Conflict).
- Delete `Controllers/AccountController.cs`.
- Success **values** preserve today's shape exactly — `{ user: <User with masked password>, token: "<jwt>", expiresIn: "<ISO-8601>" }` — but they are now nested at `response.value` of the `Result<LoginResponse>` / `Result<RegisterResponse>` envelope (the body root has `isSuccess`, `isFailure`, `error`, `value`).
</requirements>

## Subtasks

- [ ] 5.1 Create `Features/Account/Shared/AccountErrors.cs`.
- [ ] 5.2 Create the `Login` operation folder with all five files.
- [ ] 5.3 Create the `Register` operation folder with all five files.
- [ ] 5.4 Delete `Controllers/AccountController.cs`.
- [ ] 5.5 Manual Verification.

## Implementation Details

See `techspec.md` → "Implementation Design → Main Interfaces" (handler + endpoint examples) and "API Endpoints" (account row) for exact routes and auth. The fields inside `Result<T>.Value` (the `LoginResponse` / `RegisterResponse` records) MUST serialize to the same JSON keys (casing/naming) as today's bare body — clients reading `response.value` will see the same shape.

## Success Criteria

- `Features/Account/` exists with `Login` and `Register` operation folders.
- `Controllers/AccountController.cs` no longer exists.
- `POST /v1/accounts/login` and `POST /v1/accounts/register` continue to succeed at status 200 and return the `Result<LoginResponse>` / `Result<RegisterResponse>` envelope — `response.value` matches the pre-refactor body byte-for-byte (diff a captured `value` from before/after).
- Error responses use the `Result` envelope (`{ isSuccess: false, isFailure: true, error: { code, description, type } }`), not ProblemDetails. This is an intentional, PRD-sanctioned change.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Happy path: `POST /v1/accounts/register` with a fresh email + valid fields → 200, body `{ isSuccess: true, isFailure: false, error: { code: "", description: "" }, value: { user, token, expiresIn } }`; JWT (read from `response.value.token`) decodes to the same claims as before (`NameIdentifier`, `Name`, `Role: "Cyclist"`).
  - [ ] Duplicate email: second register call with same email → 409 with body `{ isSuccess: false, isFailure: true, error: { code: "Account.EmailAlreadyExists", description: "...", type: "Conflict" } }`.
  - [ ] Login happy path: `POST /v1/accounts/login` with correct credentials → 200 with the `Result<LoginResponse>` envelope; `response.value` matches today's bare-body shape.
  - [ ] Login with wrong password → 404 with `{ isSuccess: false, error: { code: "Account.InvalidCredentials", type: "NotFound" } }`.
  - [ ] Login with missing email → 400 with the validation Result envelope: `{ isSuccess: false, errors: [{ code: "Email", description: "...", type: "Validation" }, ...] }`.
  - [ ] Register with phone in wrong format → 400 validation Result envelope with an `errors[]` entry whose `code: "Phone"`.
  - [ ] Freshly registered user can call `GET /v1/bikes` with the token read from `response.value.token` → 200.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Account/Shared/AccountErrors.cs` (new)
- `Features/Account/Login/*` (new — 5 files)
- `Features/Account/Register/*` (new — 5 files)
- `Controllers/AccountController.cs` (deleted)
