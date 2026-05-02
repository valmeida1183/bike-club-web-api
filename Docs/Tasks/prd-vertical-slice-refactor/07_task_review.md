# Review: Task 7.0 - Lookup Features Slice (Category, Difficulty, Gender, Role)

**Reviewer**: AI Code Reviewer
**Date**: 2026-05-02
**Task File**: 07_task.md
**Status**: APPROVED WITH OBSERVATIONS

## Summary

All 17 endpoint classes covering four lookup feature slices (Category: 5, Difficulty: 5, Gender: 3, Role: 4) were implemented under `Features/`. The four original controllers were deleted. The build is clean (0 errors, 0 warnings). All task requirements are met: routes, verbs, auth rules, output caching on the three anonymous GET-list endpoints, IdMismatch on Category/Difficulty PUT, NameMismatch on Role PUT with `OrdinalIgnoreCase`, non-generic `Result` on DELETE operations, and the full `Result<T>` envelope contract. No critical issues were found. Two minor observations are noted below.

## Reviewed Files

| File | Status | Issues |
|------|--------|--------|
| `Features/Category/Shared/CategoryErrors.cs` | OK | 0 |
| `Features/Category/Shared/CategoryResponse.cs` | OK | 0 |
| `Features/Category/Shared/ICategoryRequest.cs` | OK | 0 |
| `Features/Category/Shared/CategoryRequestValidator.cs` | OK | 0 |
| `Features/Category/GetCategory/GetCategoryHandler.cs` | OK | 0 |
| `Features/Category/GetCategory/GetCategoryEndpoint.cs` | OK | 0 |
| `Features/Category/GetCategoryById/GetCategoryByIdHandler.cs` | OK | 0 |
| `Features/Category/GetCategoryById/GetCategoryByIdEndpoint.cs` | OK | 0 |
| `Features/Category/CreateCategory/CreateCategoryRequest.cs` | OK | 0 |
| `Features/Category/CreateCategory/CreateCategoryValidator.cs` | OK | 0 |
| `Features/Category/CreateCategory/CreateCategoryHandler.cs` | OK | 0 |
| `Features/Category/CreateCategory/CreateCategoryEndpoint.cs` | OK | 0 |
| `Features/Category/UpdateCategory/UpdateCategoryRequest.cs` | OK | 0 |
| `Features/Category/UpdateCategory/UpdateCategoryValidator.cs` | OK | 0 |
| `Features/Category/UpdateCategory/UpdateCategoryHandler.cs` | OK | 0 |
| `Features/Category/UpdateCategory/UpdateCategoryEndpoint.cs` | OK | 0 |
| `Features/Category/DeleteCategory/DeleteCategoryHandler.cs` | OK | 0 |
| `Features/Category/DeleteCategory/DeleteCategoryEndpoint.cs` | OK | 0 |
| `Features/Difficulty/Shared/DifficultyErrors.cs` | OK | 0 |
| `Features/Difficulty/Shared/DifficultyResponse.cs` | OK | 0 |
| `Features/Difficulty/Shared/IDifficultyRequest.cs` | OK | 0 |
| `Features/Difficulty/Shared/DifficultyRequestValidator.cs` | OK | 0 |
| `Features/Difficulty/GetDifficulty/GetDifficultyHandler.cs` | OK | 0 |
| `Features/Difficulty/GetDifficulty/GetDifficultyEndpoint.cs` | OK | 0 |
| `Features/Difficulty/GetDifficultyById/GetDifficultyByIdHandler.cs` | OK | 0 |
| `Features/Difficulty/GetDifficultyById/GetDifficultyByIdEndpoint.cs` | OK | 0 |
| `Features/Difficulty/CreateDifficulty/CreateDifficultyRequest.cs` | OK | 0 |
| `Features/Difficulty/CreateDifficulty/CreateDifficultyValidator.cs` | OK | 0 |
| `Features/Difficulty/CreateDifficulty/CreateDifficultyHandler.cs` | OK | 0 |
| `Features/Difficulty/CreateDifficulty/CreateDifficultyEndpoint.cs` | OK | 0 |
| `Features/Difficulty/UpdateDifficulty/UpdateDifficultyRequest.cs` | OK | 0 |
| `Features/Difficulty/UpdateDifficulty/UpdateDifficultyValidator.cs` | OK | 0 |
| `Features/Difficulty/UpdateDifficulty/UpdateDifficultyHandler.cs` | OK | 0 |
| `Features/Difficulty/UpdateDifficulty/UpdateDifficultyEndpoint.cs` | OK | 0 |
| `Features/Difficulty/DeleteDifficulty/DeleteDifficultyHandler.cs` | OK | 0 |
| `Features/Difficulty/DeleteDifficulty/DeleteDifficultyEndpoint.cs` | OK | 0 |
| `Features/Gender/Shared/GenderErrors.cs` | OK | 0 |
| `Features/Gender/Shared/GenderResponse.cs` | OK | 0 |
| `Features/Gender/GetGender/GetGenderHandler.cs` | OK | 0 |
| `Features/Gender/GetGender/GetGenderEndpoint.cs` | OK | 0 |
| `Features/Gender/GetGenderByCode/GetGenderByCodeHandler.cs` | OK | 0 |
| `Features/Gender/GetGenderByCode/GetGenderByCodeEndpoint.cs` | OK | 0 |
| `Features/Gender/CreateGender/CreateGenderRequest.cs` | OK | 0 |
| `Features/Gender/CreateGender/CreateGenderValidator.cs` | OK | 0 |
| `Features/Gender/CreateGender/CreateGenderHandler.cs` | OK | 0 |
| `Features/Gender/CreateGender/CreateGenderEndpoint.cs` | OK | 0 |
| `Features/Role/Shared/RoleErrors.cs` | OK | 0 |
| `Features/Role/Shared/RoleResponse.cs` | OK | 0 |
| `Features/Role/Shared/IRoleRequest.cs` | OK | 0 |
| `Features/Role/Shared/RoleRequestValidator.cs` | OK | 0 |
| `Features/Role/GetRole/GetRoleHandler.cs` | OK | 0 |
| `Features/Role/GetRole/GetRoleEndpoint.cs` | OK | 0 |
| `Features/Role/GetRoleByName/GetRoleByNameHandler.cs` | OK | 0 |
| `Features/Role/GetRoleByName/GetRoleByNameEndpoint.cs` | OK | 0 |
| `Features/Role/CreateRole/CreateRoleRequest.cs` | OK | 0 |
| `Features/Role/CreateRole/CreateRoleValidator.cs` | OK | 0 |
| `Features/Role/CreateRole/CreateRoleHandler.cs` | OK | 0 |
| `Features/Role/CreateRole/CreateRoleEndpoint.cs` | OK | 0 |
| `Features/Role/UpdateRole/UpdateRoleRequest.cs` | OK | 0 |
| `Features/Role/UpdateRole/UpdateRoleValidator.cs` | OK | 0 |
| `Features/Role/UpdateRole/UpdateRoleHandler.cs` | OK | 1 minor |
| `Features/Role/UpdateRole/UpdateRoleEndpoint.cs` | OK | 0 |
| `Controllers/CategoryController.cs` | OK (deleted) | 0 |
| `Controllers/DifficultyController.cs` | OK (deleted) | 0 |
| `Controllers/GenderController.cs` | OK (deleted) | 0 |
| `Controllers/RoleController.cs` | OK (deleted) | 0 |

## Issues Found

### Critical Issues

No critical issues found.

### Major Issues

No major issues found.

### Minor Issues

**MINOR-1 — `UpdateRoleHandler` does not verify the role exists before applying `EntityState.Modified`**

File: `Features/Role/UpdateRole/UpdateRoleHandler.cs`, lines 33-35

```csharp
var role = new RoleEntity { Name = request.Name, Description = request.Description };
_db.Entry(role).State = EntityState.Modified;
await _db.SaveChangesAsync(ct);
```

When `Name` is the entity's primary key and the row does not exist in the database, EF Core will throw a `DbUpdateConcurrencyException` at `SaveChangesAsync`. This is the same pattern used in the original `RoleController.Put` and in the Address and Category/Difficulty UPDATE handlers from this project, so it is a deliberate project-wide convention (behavior is preserved by design, not introduced as a regression). However, the failure mode at runtime is an unhandled exception rather than a structured `Role.NotFound` result, which means the caller receives a 500 instead of a 404 for a missing role name on PUT.

This was explicitly acknowledged as a known risk in the techspec ("Known Risks: Missing row on PUT throws `DbUpdateConcurrencyException`") and is out of scope for this task, but it is worth tracking so a future task can add the existence check uniformly across all UPDATE handlers.

No code change is required for this review cycle.

---

**MINOR-2 — `UpdateRoleRequest` does not carry an `Id` property, but the `IRoleRequest` interface and `RoleRequestValidator` validate `Name` as a required, length-constrained field**

File: `Features/Role/UpdateRole/UpdateRoleRequest.cs`, line 5
File: `Features/Role/Shared/RoleRequestValidator.cs`, lines 9-12

`UpdateRoleRequest` intentionally omits a separate `Id` property because Role's primary key is its `Name`. The NameMismatch check compares the route `{name}` against `request.Name` using `OrdinalIgnoreCase` — meaning validation still passes when route and body names differ only in case (e.g., `PUT /v1/roles/cyclist` with body `{ "name": "Cyclist" }` succeeds). This matches the original controller behavior exactly and the task requirement to preserve `OrdinalIgnoreCase`. No defect here.

Flagged only as a documentation point: the NameMismatch guard allows case-normalized renames of the display form (`"cyclist"` → `"Cyclist"`) without being flagged as a mismatch, which may surprise a future developer. A code comment would help, but the code standards for this project discourage comments in favor of self-explaining code, so no change is required.

## Positive Highlights

- The `using CategoryEntity = BikeClub.Domain.Entities.Category` (and equivalent aliases for Difficulty, Gender, Role) cleanly resolve the namespace collision between each `Domain.Entities` type and its `Features.<Feature>` namespace without renaming entities or restructuring folders. This is a consistent application of the alias pattern established in task 6.
- Output caching is applied inline via `.CacheOutput(b => b.Expire(TimeSpan.FromSeconds(30)).SetVaryByHeader("User-Agent"))` on exactly the three endpoints that require it (`GET /v1/categories`, `GET /v1/difficulties`, `GET /v1/genders`), matching the task requirement and the techspec rationale.
- Role GET endpoints use `.RequireAuthorization()` (any authenticated user), while Role POST and PUT use `.RequireAuthorization(new AuthorizeAttribute { Roles = RoleStatic.Monitor })`. This correctly preserves the original controller's authorization model: any authenticated principal can read roles, but only Monitor can write.
- `UpdateRoleHandler.Handle` checks `NameMismatch` using `StringComparison.OrdinalIgnoreCase`, exactly matching the original controller's comparison mode and the explicit task requirement to preserve that behavior.
- Gender feature correctly implements only 3 operations (GET list, GET by code, POST) without adding PUT or DELETE, matching the original `GenderController` and the task's explicit "do not add them" constraint.
- All handler and endpoint classes are consistently marked `internal sealed`, preventing accidental external consumption and enabling minor JIT optimizations.
- `GetCategoryByIdHandler` and `GetDifficultyByIdHandler` both return `CategoryErrors.NotFound` / `DifficultyErrors.NotFound` (404) on a missing record, improving on the original controllers that silently returned `200 OK` with a null body.
- `ICategoryRequest`, `IDifficultyRequest`, and `IRoleRequest` are each in their own file in the `Shared/` folder, following the one-type-per-file convention. This is a direct improvement over task 6 where `IAddressRequest` was still embedded in `AddressRequestValidator.cs` at the time of that review.
- The `DeleteCategoryHandler` and `DeleteDifficultyHandler` return `Result.Failure(error)` explicitly, which is the correct and only approach for non-generic `Result` (which has no implicit operator from `Error`). The difference in syntax from the generic `Result<T>` handlers is mandated by the type system and is correct.
- `AsNoTracking()` is used on all read-only queries, and `FirstOrDefaultAsync` is used without `AsNoTracking()` in DELETE handlers so EF has a tracked entity to `Remove`. This distinction is applied correctly and consistently across all four feature slices.
- The `CategoryRequestValidator<T>` and `DifficultyRequestValidator<T>` classes use a single-field, single-constraint-per-rule structure that maps exactly to the original entity data annotations, ensuring validation messages are preserved byte-for-byte.

## Standards Compliance

| Standard | Status |
|----------|--------|
| Code Standards (naming, size, nesting, no magic numbers) | OK |
| English — all identifiers and strings in English | OK |
| REST/HTTP — routes, verbs, auth, status codes per task spec | OK |
| Result pattern — Result/Result<T>, ValidationResult, Error | OK |
| Minimal-API — IEndpoint, one class per operation, thin endpoint | OK |
| Build (dotnet build) | OK (0 errors, 0 warnings) |
| Tests | N/A (manual verification per PRD) |

## Recommendations

1. Execute the full manual verification checklist from the task file before marking complete. Priority scenarios: `GET /v1/categories` (verify `Age` header and caching within 30s); anonymous `GET /v1/roles` (must return 401); `PUT /v1/difficulties/1` with body `{ "id": 2, ... }` (must return 400 with `error.code: "Difficulty.IdMismatch"`); `PUT /v1/roles/Cyclist` with body `{ "name": "Monitor", ... }` (must return 400 with `error.code: "Role.NameMismatch"`); DELETE on a missing id (must return 404 with Result envelope); DELETE on an existing record (must return 204 with no body).
2. Track the missing-row-on-PUT behavior (MINOR-1) as a follow-up item across all UPDATE handlers in the project. When addressed, a single `FindAsync` before `EntityState.Modified` and returning the appropriate `NotFound` error will unify the error surface.

## Verdict

The implementation is correct and production-ready. All 17 endpoint slices match the task requirements, the Result envelope contract is applied uniformly, output caching is applied exactly where required, auth rules are faithfully preserved from the original controllers, and all four controllers are deleted. The two minor observations are either by-design behavioral carryovers or informational notes requiring no code changes. Complete the manual verification checklist before marking the task done.
