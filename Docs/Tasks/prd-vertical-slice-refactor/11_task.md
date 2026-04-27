# Task 11.0: Purchase Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `PurchaseController`. Two operations: `CreatePurchase` (POST — with upsert-on-existing semantics) and `DeletePurchase` (DELETE). Note the unusual upsert behavior on POST: if a Purchase already exists for the same `(ShopCartId, BikeId)`, the handler increments its `Quantity` instead of inserting a duplicate. Preserve that exactly.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**, **`minimal-api`**.
</skills>

<requirements>
- `Features/Purchase/CreatePurchase/` — `POST v1/purchases`, `RequireAuthorization()`. Returns `Result<PurchaseResponse>`.
  - Validator mirrors `Purchase` entity annotations.
  - Handler: look up existing `Purchase` by `(ShopCartId, BikeId)` with `AsNoTracking().FirstOrDefaultAsync`.
    - If found: increment `Quantity += request.Quantity`, `context.Purchases.Update(existing)`, save, return `Result.Success(updatedPurchase)`.
    - If not: `context.Purchases.Add(new)`, save, return `Result.Success(newPurchase)`.
  - Success **value** matches today's `currentPurchase ?? purchase` body, now nested at `response.value` of the `Result<PurchaseResponse>` envelope.
- `Features/Purchase/DeletePurchase/` — `DELETE v1/purchases/{id:int}`, `RequireAuthorization()`. Handler returns non-generic `Result`.
  - If not found → `Result.Failure(PurchaseErrors.NotFound)` (`ErrorType.NotFound`) → 404 with the Result envelope (today returns bare `NotFound()` — the new behavior is a 404 with the Result envelope, which is the PRD-sanctioned response contract change).
  - On success: handler returns `Result.Success()`; endpoint returns `204 NoContent` (no body). Today's `Ok(new { message = "Purchase removed with success." })` body is dropped because the verb does not produce a body under the new convention. If the team requires preserving the message, override the endpoint with `.ToIResult(onSuccess: r => Results.Ok(r))` and switch the handler to `Result<DeletePurchaseResponse>` carrying the message — explicit per-task decision.
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
  - [ ] `POST /v1/purchases` with `{ shopCartId: X, bikeId: Y, quantity: 2 }` → 200 with `Result<PurchaseResponse>` envelope; `response.value` is the new purchase.
  - [ ] Same POST again → 200, `response.value.quantity` is 4 (incremented, not duplicated).
  - [ ] Verify in DB: only one row for `(X, Y)` exists after both calls.
  - [ ] `POST /v1/purchases` with invalid body (e.g., missing bikeId) → 400 with validation Result envelope (`errors[]` populated).
  - [ ] `DELETE /v1/purchases/{existingId}` → 204 NoContent (no body, per new convention).
  - [ ] `DELETE /v1/purchases/9999` → 404 with Result envelope, `error.code: "Purchase.NotFound"`, `error.type: "NotFound"`.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/Purchase/*` (new)
- `Controllers/PurchaseController.cs` (deleted)
