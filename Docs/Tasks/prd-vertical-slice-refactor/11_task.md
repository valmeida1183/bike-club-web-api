# Task 11.0: Purchase Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `PurchaseController`. Two operations: `CreatePurchase` (POST — with upsert-on-existing semantics) and `DeletePurchase` (DELETE). Note the unusual upsert behavior on POST: if a Purchase already exists for the same `(ShopCartId, BikeId)`, the handler increments its `Quantity` instead of inserting a duplicate. Preserve that exactly.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**, **`minimal-api`**.
</skills>

<requirements>
- `Features/Purchase/CreatePurchase/` — `POST v1/purchases`, `RequireAuthorization()`.
  - Validator mirrors `Purchase` entity annotations.
  - Handler: look up existing `Purchase` by `(ShopCartId, BikeId)` with `AsNoTracking().FirstOrDefaultAsync`.
    - If found: increment `Quantity += request.Quantity`, `context.Purchases.Update(existing)`, save, return the **updated** purchase.
    - If not: `context.Purchases.Add(new)`, save, return the **new** purchase.
  - Success payload matches today's: `Ok(currentPurchase ?? purchase)`.
- `Features/Purchase/DeletePurchase/` — `DELETE v1/purchases/{id:int}`, `RequireAuthorization()`.
  - If not found → 404 ProblemDetails (today returns bare `NotFound()` — the new behavior is a 404 ProblemDetails envelope, which is the PRD-sanctioned error contract change).
  - On success: `Ok(new { message = "Purchase removed with success." })`.
- Shared `PurchaseErrors.cs` with `NotFound`.
- Delete `Controllers/PurchaseController.cs`.
</requirements>

## Subtasks

- [ ] 11.1 Shared errors + validator.
- [ ] 11.2 `CreatePurchase` (upsert logic).
- [ ] 11.3 `DeletePurchase`.
- [ ] 11.4 Delete `Controllers/PurchaseController.cs`.
- [ ] 11.5 Manual Verification.

## Implementation Details

See `techspec.md` → "API Endpoints" (purchases row). The upsert logic uses `AsNoTracking()` on the lookup then `Update()` — verify this still works with EF change-tracking after migrating (EF `Update` re-attaches the entity as Modified, which is the intended behavior).

## Success Criteria

- Two routes migrated; `Controllers/PurchaseController.cs` gone.
- Upsert semantics preserved: repeated POSTs with identical `(ShopCartId, BikeId)` increment quantity, not insert.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] `POST /v1/purchases` with `{ shopCartId: X, bikeId: Y, quantity: 2 }` → 200, new purchase returned.
  - [ ] Same POST again → 200, returned purchase has `quantity: 4` (incremented, not duplicated).
  - [ ] Verify in DB: only one row for `(X, Y)` exists after both calls.
  - [ ] `POST /v1/purchases` with invalid body (e.g., missing bikeId) → 400 ValidationProblem.
  - [ ] `DELETE /v1/purchases/{existingId}` → 200 `{ message: "Purchase removed with success." }`.
  - [ ] `DELETE /v1/purchases/9999` → 404 ProblemDetails.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Purchase/*` (new)
- `Controllers/PurchaseController.cs` (deleted)
