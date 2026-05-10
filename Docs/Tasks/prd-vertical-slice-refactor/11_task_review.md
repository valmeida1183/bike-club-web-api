# Review: Task 11 - Purchase Feature Slice

**Reviewer**: AI Code Reviewer
**Date**: 2026-05-10
**Task File**: 11_task.md
**Status**: CHANGES REQUESTED

## Summary

The `CreatePurchase` slice is well-implemented: the upsert logic faithfully mirrors the original controller, the validator correctly maps the entity annotations, the `IEndpoint`/`ToIResult()` pattern matches all other features, and the `PurchaseEntity` alias cleanly resolves the namespace/type collision. The build passes with 0 errors and 0 warnings.

However, the task requires **two** routes to be migrated and specifies `DeletePurchase` as a mandatory subtask (11.3) with its own success criteria, `PurchaseErrors.cs`, manual-verification steps, and an explicit route signature. Only one route was delivered. The `Controllers/PurchaseController.cs` deletion is therefore premature while the deletion endpoint has no replacement. This is a spec-deviation, not a style issue, and it blocks sign-off regardless of the verbal approval obtained during development.

---

## Reviewed Files

| File | Status | Issues |
|------|--------|--------|
| `Features/Purchase/Shared/PurchaseResponse.cs` | OK | 0 |
| `Features/Purchase/CreatePurchase/CreatePurchaseRequest.cs` | OK | 0 |
| `Features/Purchase/CreatePurchase/CreatePurchaseValidator.cs` | OK | 0 |
| `Features/Purchase/CreatePurchase/CreatePurchaseHandler.cs` | OK | 0 |
| `Features/Purchase/CreatePurchase/CreatePurchaseEndpoint.cs` | OK | 0 |
| `Features/Purchase/` (directory) | Critical | Missing `DeletePurchase/` subtree and `Shared/PurchaseErrors.cs` |

---

## Issues Found

### Critical Issues

**[C-1] `DeletePurchase` endpoint is absent — task success criteria not met**

- Location: `Features/Purchase/` (directory missing `DeletePurchase/` and `Shared/PurchaseErrors.cs`)
- Task 11_task.md states:
  - Subtask 11.3: implement `DeletePurchase`.
  - Success Criteria: "Two routes migrated; `Controllers/PurchaseController.cs` gone."
  - Required files: `Features/Purchase/DeletePurchase/DeletePurchaseEndpoint.cs`, `DeletePurchaseHandler.cs`, `Shared/PurchaseErrors.cs`.
  - Manual verification: `DELETE /v1/purchases/{existingId}` → 204; `DELETE /v1/purchases/9999` → 404 with Result envelope.
- PRD §3.2: "All endpoints MUST preserve their current route path, HTTP verb."
- PRD §8.4: "Features MUST be migrated one at a time. Each migration MUST delete the corresponding controller only **once all its endpoints have moved**."
- The `DELETE v1/purchases/{id:int}` route existed in the original `PurchaseController`; by deleting the controller without a replacement the route has been dropped from the API surface, which violates the PRD.

The composite-key fact is confirmed: `PurchaseConfiguration.cs` declares `builder.HasKey(p => new { p.BikeId, p.ShopCartId })`. The original `FindAsync(id)` was indeed broken. However, "broken original" is a reason to fix the route signature, not to drop the endpoint. The correct remediation is to implement the handler using the composite key, matching the pattern already present in `ShopCartController.RemovePurchaseFromShopCart`:

```csharp
// Features/Purchase/DeletePurchase/DeletePurchaseHandler.cs
public async Task<Result> Handle(int shopCartId, int bikeId, CancellationToken ct)
{
    var purchase = await _db.Purchases
        .FirstOrDefaultAsync(p => p.ShopCartId == shopCartId && p.BikeId == bikeId, ct);

    if (purchase is null)
        return Result.Failure(PurchaseErrors.NotFound);

    _db.Purchases.Remove(purchase);
    await _db.SaveChangesAsync(ct);

    return Result.Success();
}
```

The task already sanctions a 204 NoContent on success (non-generic `Result.Success()` flows through `ToIResult()` to `Results.NoContent()`), matching the other delete endpoints (e.g., `DeleteBikeEndpoint`).

If the team's explicit decision is to formally drop the endpoint (accepting the PRD deviation), then the correct process is:
1. Update `11_task.md` and `tasks.md` to record the scope change and rationale.
2. Confirm with the tech lead that the PRD §3.2 / §8.4 constraint is waived for this endpoint.
3. Re-submit for review with the updated scope documents.

Until one of these two paths is followed, the implementation does not satisfy the task definition.

---

### Major Issues

No major issues found. All code-standards items pass.

---

### Minor Issues

No minor issues found.

---

## Positive Highlights

- **Upsert logic is a faithful port.** The `AsNoTracking().FirstOrDefaultAsync` lookup followed by `context.Purchases.Update(existing)` exactly mirrors the original controller pattern and the task's implementation note. The re-attach-as-Modified behavior of EF `Update` is correctly leveraged.

- **Namespace alias is the right fix.** Using `PurchaseEntity = BikeClub.Domain.Entities.Purchase` at the file level to disambiguate the feature namespace from the entity type is clean and consistent with the `BikeEntity` alias used in `CreateBikeHandler.cs`.

- **Validator correctly transcribes entity annotations.** `[Required] int ShopCartId` → `GreaterThan(0)`; `[Required] int BikeId` → `GreaterThan(0)`; `[Required, Range(1, int.MaxValue)] int Quantity` → `GreaterThanOrEqualTo(1)`. All three rules match.

- **Endpoint is minimal and consistent.** `CreatePurchaseEndpoint.cs` follows the identical shape used across all other feature endpoints (`IEndpoint`, single-line lambda, `.RequireAuthorization()`, `.WithTags(...)`). No deviation.

- **Build is clean.** 0 errors, 0 warnings confirms the implementation integrates without friction into the existing project.

- **Implicit operator used cleanly.** `return PurchaseResponse.From(existing)` works because `Result<PurchaseResponse>` carries an implicit conversion from `PurchaseResponse` — the same pattern used in `CreateBikeHandler.cs` and across the codebase.

---

## Standards Compliance

| Standard | Status |
|----------|--------|
| Code Standards (naming, nesting, size, no magic numbers) | OK |
| .NET / ASP.NET Core Minimal API conventions | OK |
| REST / HTTP semantics | OK — for the delivered endpoint |
| FluentValidation usage | OK |
| Result pattern (`result-pattern` skill) | OK |
| Tests (N/A per PRD) | N/A |
| Task completeness (all subtasks delivered) | Fail — subtask 11.3 missing |

---

## Recommendations

1. **[Required]** Implement `Features/Purchase/DeletePurchase/` with `DeletePurchaseHandler.cs` and `DeletePurchaseEndpoint.cs`, using the composite key `(shopCartId, bikeId)` as route parameters (`DELETE v1/purchases/{shopCartId:int}/{bikeId:int}`) and a shared `Features/Purchase/Shared/PurchaseErrors.cs`. This is the path that satisfies the task and PRD without further approvals.

2. **[Alternative — if the scope change is retained]** Document the decision formally: update `11_task.md` to strike subtask 11.3 and adjust the success criteria, record the rationale (broken original `FindAsync` on composite PK), and obtain tech lead sign-off before treating the task as done.

3. **[Informational]** The `[Required]` / `[Range]` annotations on `Domain/Entities/Purchase.cs` are still present. Removal is out of scope for this task (deferred to Task 13 Cleanup) — no action needed here.

---

## Verdict

CHANGES REQUESTED. The `CreatePurchase` slice is correct and production-ready. The `DeletePurchase` slice is absent. The task definition requires two routes and lists `DeletePurchase` as a mandatory subtask with its own success criteria. Either implement the missing slice or formally amend the task scope with stakeholder approval before re-submitting.
