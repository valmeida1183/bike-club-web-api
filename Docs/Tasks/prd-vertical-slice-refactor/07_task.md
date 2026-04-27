# Task 7.0: Lookup Features Slice (Category, Difficulty, Gender, Role)

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate the four simple lookup controllers together because they share the same shape: anonymous GETs with `[ResponseCache(VaryByHeader = "User-Agent", Location = Any, Duration = 30)]` (where applicable) and Monitor-only writes. Three of these (Category, Difficulty, Gender) currently use `ResponseCache`; they must be ported to Output Caching via `.CacheOutput(...)` using the middleware added in task 3.0. Role does not use response caching.

<skills>
### Compliance with Standard Skills

- **`result-pattern`** — per-operation `Result<T>` / `Result`. Endpoints serialize the envelope (`Result<T>` → 200 body, non-generic `Result` success → 204 NoContent, failures → status driven by `Error.Type` with the Result envelope as body).
- **`minimal-api`** — one endpoint per operation; mapping via `.ToIResult()`.
</skills>

<requirements>
- Create feature folders: `Features/Category/`, `Features/Difficulty/`, `Features/Gender/`, `Features/Role/`.

**Category**
- `GET v1/categories` — `AllowAnonymous`, **Output-cache** `VaryByHeader = "User-Agent"`, expire 30s.
- `GET v1/categories/{id:int}` — `AllowAnonymous`, returns 404 on missing (behavior improvement — see task 6.0).
- `POST v1/categories` — Monitor.
- `PUT v1/categories/{id:int}` — Monitor. Preserve "cannot change Id" → `CategoryErrors.IdMismatch`.
- `DELETE v1/categories/{id:int}` — Monitor.

**Difficulty**
- Same surface/rules as Category (`v1/difficulties`). Preserve IdMismatch on PUT.

**Gender**
- `GET v1/genders` — `AllowAnonymous`, Output-cache 30s with `VaryByHeader = "User-Agent"`.
- `GET v1/genders/{code}` — `AllowAnonymous`.
- `POST v1/genders` — Monitor.
- (Note: today's `GenderController` has no `PUT`/`DELETE` — do not add them.)

**Role**
- `GET v1/roles` — `RequireAuthorization()` (NOT anonymous; confirmed from today's controller).
- `GET v1/roles/{name}` — `RequireAuthorization()`.
- `POST v1/roles` — Monitor.
- `PUT v1/roles/{name}` — Monitor. Preserve "cannot change Name" → `RoleErrors.NameMismatch`. Note the current controller uses `StringComparison.OrdinalIgnoreCase` — preserve that behavior.

- Per feature, create `Features/<Feature>/Shared/<Feature>Errors.cs` and shared request validators where rules are reused across Create/Update.
- Delete `Controllers/CategoryController.cs`, `Controllers/DifficultyController.cs`, `Controllers/GenderController.cs`, `Controllers/RoleController.cs` once each feature is migrated.
- Output caching policy: use the default policy from task 3.0 if its defaults are a superset of `VaryByHeader = "User-Agent"` + 30s expire; otherwise define per-endpoint policy inline via `.CacheOutput(b => b.Expire(TimeSpan.FromSeconds(30)).SetVaryByHeader("User-Agent"))`.
</requirements>

## Subtasks

- [ ] 7.1 Create `Features/Category/` (Shared + 5 operations); delete `CategoryController`; verify `.CacheOutput` on GET list.
- [ ] 7.2 Create `Features/Difficulty/` (Shared + 5 operations); delete `DifficultyController`; verify `.CacheOutput` on GET list.
- [ ] 7.3 Create `Features/Gender/` (Shared + 3 operations); delete `GenderController`; verify `.CacheOutput` on GET list.
- [ ] 7.4 Create `Features/Role/` (Shared + 4 operations); delete `RoleController`.
- [ ] 7.5 Manual Verification (per sub-feature checklist below).

## Implementation Details

See `techspec.md` → "Known Risks → ResponseCache on anonymous lookup GETs" for the caching rationale.

Success **values** must stay byte-identical to the current controllers (field names, nullable handling, list ordering — current controllers do not `OrderBy`; neither should the new handlers). Those values now live at `response.value` of the `Result<T>` envelope; clients reading `response.value` see exactly today's body shape. For DELETE operations whose handler returns non-generic `Result`, success is `204 NoContent` (no body); failures still carry the Result envelope.

## Success Criteria

- Four controllers deleted.
- Eighteen new `IEndpoint` classes registered (5 Category + 5 Difficulty + 3 Gender + 4 Role + Role has no DELETE + Role has no GET anon — count = 5 + 5 + 3 + 4 = **17**; validate against the today's controller count).
- All happy-path responses use the same status code as today; the success **value** read from `response.value` matches today's bare body byte-for-byte.
- `Cache-Control` / `Vary: User-Agent` headers appear on `GET /v1/genders`, `GET /v1/categories`, `GET /v1/difficulties` responses.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] Anon: `GET /v1/categories` → 200 with `Result<…>` envelope (`response.value` is the list), cached on second call within 30s (same `Age` header or short round-trip).
  - [ ] Anon: `GET /v1/difficulties` and `GET /v1/genders` — same caching behavior; body is the Result envelope.
  - [ ] Cyclist token: `GET /v1/roles` → 200 Result envelope. Anonymous: `GET /v1/roles` → 401 (no body).
  - [ ] Monitor: `POST /v1/categories` valid body → 200 Result envelope (`response.value` = created Category). Cyclist: same request → 403 (no body).
  - [ ] Monitor: `PUT /v1/difficulties/1` with body `{ "id": 2, ... }` → 400 with `Result` envelope, `error.code: "Difficulty.IdMismatch"`, `error.type: "Validation"`.
  - [ ] Monitor: `PUT /v1/roles/Cyclist` with body `{ "name": "Monitor", ... }` → 400 with `error.code: "Role.NameMismatch"` (case-insensitive comparison preserved).
  - [ ] `GET /v1/categories/9999` → 404 with `Result` envelope, `error.code: "Category.NotFound"`, `error.type: "NotFound"`.
  - [ ] DELETE operations on missing records → 404 Result envelope; on existing records → `204 NoContent` (no body).
  - [ ] Auto-registration: adding a new endpoint class and restarting should surface it in Swagger without any `Program.cs` edit.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Category/*` (new)
- `Features/Difficulty/*` (new)
- `Features/Gender/*` (new)
- `Features/Role/*` (new)
- `Controllers/CategoryController.cs`, `DifficultyController.cs`, `GenderController.cs`, `RoleController.cs` (deleted)
