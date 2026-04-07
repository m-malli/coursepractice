# QA Report — Online Shopping App (.NET 8)

## 1. Test Plan

| Layer | File | Coverage Target |
|---|---|---|
| Domain | `tests/ShoppingApp.Tests/Domain/OrderTests.cs` | Order.Cancel, Product.ReduceStock/EffectivePrice/HasSufficientStock, Coupon.IsValid/CalculateDiscount |
| Application | `tests/ShoppingApp.Tests/Application/ProductServiceTests.cs` | Get, Create, Update (found/not-found/partial), GetAll paged |
| Application | `tests/ShoppingApp.Tests/Application/CartServiceTests.cs` | Add (new/merge/not-found/insufficient), Update (not-in-cart), Get empty, Clear |
| Application | `tests/ShoppingApp.Tests/Application/OrderServiceTests.cs` | Create (empty cart/valid/with coupon/insufficient stock/payment fail), GetById wrong user, Cancel shipped, UpdateStatus (valid/invalid/not-found) |
| Application | `tests/ShoppingApp.Tests/Application/CouponServiceTests.cs` | Validate (not-found/expired/exhausted/valid), Create (uppercase code) |
| Application | `tests/ShoppingApp.Tests/Application/ValidatorTests.cs` | All 3 validators: valid input + each invalid field |

## 2. What Was Verified

- **Compile**: 0 errors, 0 warnings across all 5 projects
- **Domain logic**: All entity methods tested with boundary/edge cases
- **Service layer**: All service methods tested for success + failure paths
- **Validators**: All validation rules verified with valid and invalid inputs
- **Code review**: All controllers, middleware, DI wiring, mappings reviewed

## 3. Tests Added/Updated (by file)

| File | Before | After | Delta |
|---|---|---|---|
| `tests/ShoppingApp.Tests/Domain/OrderTests.cs` | 6 tests | 19 tests (+1 Theory×2) | +15 |
| `tests/ShoppingApp.Tests/Application/ProductServiceTests.cs` | 3 tests | 6 tests | +3 |
| `tests/ShoppingApp.Tests/Application/CartServiceTests.cs` | 2 tests | 7 tests | +5 |
| `tests/ShoppingApp.Tests/Application/OrderServiceTests.cs` | **new** | 10 tests | +10 |
| `tests/ShoppingApp.Tests/Application/CouponServiceTests.cs` | **new** | 5 tests | +5 |
| `tests/ShoppingApp.Tests/Application/ValidatorTests.cs` | **new** | 16 tests | +16 |
| **Total** | **11** | **55** (57 including Theory inline data) | **+46** |

## 4. Edge Cases Checked

- **Product**: stock exactly equal to request (boundary), zero discount price (EffectivePrice fallback)
- **Order.Cancel**: all 7 status values — Pending/Confirmed/Processing succeed; Shipped/Delivered throw
- **Coupon**: expired, usage-limit-exhausted, below-min-order, no-max-cap (uncapped discount)
- **Cart**: add to existing item (quantity merge), product not found, item not in cart for update
- **Order flow**: empty cart reject, payment failure propagation, coupon discount applied & usage incremented, insufficient stock mid-checkout
- **Validators**: zero/negative/over-limit quantities, empty GUIDs, invalid emails, short passwords, empty required fields

## 5. Bugs Found & Fixed

| # | Severity | Issue | Fix |
|---|---|---|---|
| 1 | **High** | `OrderService.CancelOrderAsync` let `InvalidOperationException` from `Order.Cancel()` propagate unhandled when cancelling a shipped/delivered order. API would return 500 instead of a proper 400 error. | Added `try/catch` around `order.Cancel()` in `src/ShoppingApp.Application/Services/OrderService.cs`, now returns `ServiceResult.Fail(ex.Message)`. |

## 6. Risks / Follow-ups

| Risk | Priority | Notes |
|---|---|---|
| No concurrency token on `Product.StockQuantity` | Medium | Two concurrent checkouts could oversell. Add `[ConcurrencyCheck]` or row version. |
| JWT key is a placeholder in appsettings.json | High | Must use `dotnet user-secrets` or env vars before any deployment. |
| `UpdateAsync` DiscountPrice — setting `null` is impossible | Low | `if (dto.DiscountPrice.HasValue)` means you can set a discount but never clear it. Needs a separate "clear discount" flag or sentinel value. |
| No integration tests for controllers | Medium | Add `WebApplicationFactory` tests in a follow-up. |
| `AddToCartAsync` doesn't check combined stock | Low | Checks `product.HasSufficientStock(dto.Quantity)` but not existing cart quantity + new quantity vs available stock. |

## 7. Commands to Run Locally

```bash
cd c:\Learning\coursepractice
dotnet build ShoppingApp.sln
dotnet test ShoppingApp.sln --verbosity normal
```

## 8. Final Status

**Pass with notes** — 0 compile errors, 55+ test cases covering domain, services, and validators. One bug found and fixed (cancel-shipped exception leak). Five follow-up items identified for hardening.
