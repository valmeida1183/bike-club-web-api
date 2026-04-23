# Task 12.0: ShopCart Feature Slice

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Migrate `ShopCartController` — the most complex feature. Six operations with non-trivial EF include graphs and a shared `CalculateTotalAmount` helper that recomputes the cart total after any mutation. Introduce `Features/ShopCart/Shared/TotalAmountCalculator.cs` as a feature-local service (injected into the handlers that need it) so the logic is not duplicated across operations.

<skills>
### Compliance with Standard Skills

- **`result-pattern`**, **`minimal-api`**.
</skills>

<requirements>
- `Features/ShopCart/Shared/TotalAmountCalculator.cs` — port the current `CalculateTotalAmount(IEnumerable<Purchase>)` method. Register as scoped via assembly scanning or an explicit `AddScoped<ITotalAmountCalculator>` in `ServicesExtensions`.
- `Features/ShopCart/GetShopCartByUserId/` — `GET v1/shop-carts/{userId:int}`, `RequireAuthorization()`. Eager-loads `Address`, `Purchases.Bike`. Today returns `Ok(shopCart)` which is `null` when missing — preserve that (returning `Result.Success<ShopCartResponse?>(null)` is acceptable; endpoint returns 200 with null body to keep the exact current behavior). **Exception to the 404 convention** — this specific endpoint's today-behavior is load-bearing for clients (confirm with the team if unsure; default is to preserve).
- `Features/ShopCart/CreateShopCart/` — `POST v1/shop-carts`, `RequireAuthorization()`.
- `Features/ShopCart/AddPurchaseToShopCart/` — `POST v1/shop-carts/add-purchase`, `RequireAuthorization()`. Upsert-on-existing like Purchase (increment quantity if `(ShopCartId, BikeId)` exists). After save, reload ShopCart with `Include(sc => sc.Purchases).ThenInclude(p => p.Bike)` and set `TotalAmount = calculator.Calculate(shopCart.Purchases)`, save again, return the reloaded cart.
- `Features/ShopCart/UpdatePurchaseInShopCart/` — `PATCH v1/shop-carts/update-purchase/{shopCartId:int}/{bikeId:int}`, `RequireAuthorization()`. 404 if purchase missing. Update quantity, recompute total via shared calculator, save, return reloaded cart.
- `Features/ShopCart/SetShopCartAddress/` — `PATCH v1/shop-carts/{shopCartId:int}/address`, `RequireAuthorization()`. Preserve the quirky current behavior: `address.Id = shopCart.AddressId ?? 0; shopCart.Address = address; SaveChangesAsync;` then reload with includes and return.
- `Features/ShopCart/RemovePurchaseFromShopCart/` — `DELETE v1/shop-carts/remove-purchase/{shopCartId:int}/{bikeId:int}`, `RequireAuthorization()`. 404 if purchase missing. After remove, recompute total, save, return reloaded cart.
- Shared `ShopCartErrors.cs`: `PurchaseNotFound`, `ShopCartNotFound`.
- Delete `Controllers/ShopCartController.cs`.
- All routes are `RequireAuthorization()` — none require Monitor, matching today.
</requirements>

## Subtasks

- [ ] 12.1 Create `Features/ShopCart/Shared/` with `ShopCartErrors.cs` and `TotalAmountCalculator.cs` (+ interface). Register DI.
- [ ] 12.2 `GetShopCartByUserId`.
- [ ] 12.3 `CreateShopCart`.
- [ ] 12.4 `AddPurchaseToShopCart` (upsert + recompute total).
- [ ] 12.5 `UpdatePurchaseInShopCart` (recompute total).
- [ ] 12.6 `SetShopCartAddress` (preserve quirky address.Id behavior).
- [ ] 12.7 `RemovePurchaseFromShopCart` (recompute total).
- [ ] 12.8 Delete `Controllers/ShopCartController.cs`.
- [ ] 12.9 Manual Verification.

## Implementation Details

See `techspec.md` → "API Endpoints" (shop-carts row) for the route/auth table, and "Component Overview" for feature-local shared placement guidance.

The current controller chains two `SaveChangesAsync` calls per mutation (once to persist the purchase change, once to persist the recomputed total). Preserve this behavior — wrapping both in a single transaction is a refactor the PRD excludes.

## Success Criteria

- All six routes behave identically on the happy path.
- `TotalAmount` matches today's computation on any combination of purchase add/update/remove.
- `Controllers/ShopCartController.cs` gone.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification** (set up a cart with 2 bikes at known prices for a seeded user)
  - [ ] `GET /v1/shop-carts/{userId}` → 200, includes Address, Purchases with Bike details; matches today's payload exactly.
  - [ ] `POST /v1/shop-carts/add-purchase` with `{ shopCartId, bikeId, quantity: 2 }` → 200, `shopCart.totalAmount = bike.price * 2`.
  - [ ] Same POST again → quantity becomes 4; totalAmount doubles.
  - [ ] `PATCH /v1/shop-carts/update-purchase/{shopCartId}/{bikeId}` with `{ quantity: 1 }` → totalAmount becomes `bike.price * 1`.
  - [ ] `PATCH /v1/shop-carts/update-purchase/{shopCartId}/9999` → 404 ProblemDetails.
  - [ ] `PATCH /v1/shop-carts/{shopCartId}/address` with a new address body → 200, cart now has the updated Address (id preserved/reused per quirk).
  - [ ] `DELETE /v1/shop-carts/remove-purchase/{shopCartId}/{bikeId}` → 200, purchase gone, totalAmount recomputed.
  - [ ] `DELETE /v1/shop-carts/remove-purchase/{shopCartId}/9999` → 404.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Features/ShopCart/Shared/ShopCartErrors.cs` (new)
- `Features/ShopCart/Shared/ITotalAmountCalculator.cs` + `TotalAmountCalculator.cs` (new)
- `Features/ShopCart/GetShopCartByUserId/*` (new)
- `Features/ShopCart/CreateShopCart/*` (new)
- `Features/ShopCart/AddPurchaseToShopCart/*` (new)
- `Features/ShopCart/UpdatePurchaseInShopCart/*` (new)
- `Features/ShopCart/SetShopCartAddress/*` (new)
- `Features/ShopCart/RemovePurchaseFromShopCart/*` (new)
- `Extensions/ServicesExtensions.cs` (register `ITotalAmountCalculator`)
- `Controllers/ShopCartController.cs` (deleted)
