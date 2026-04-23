# Task 3.0: Extract `Program.cs` to Extension Methods

<critical>Read the prd.md and techspec.md files from this folder; if you don't read these files your task will be invalidated</critical>

## Overview

Pull every inline setup block out of `Program.cs` into dedicated extension classes under `Extensions/`. After this task, `Program.cs` is a declarative composition root: a sequence of extension calls plus the middleware pipeline and `app.Run()`. Behavior is unchanged.

<skills>
### Compliance with Standard Skills

- **`minimal-api`** — the Extensions folder convention (one class per setup concern) matches the skill's guidance on where extension methods live.
</skills>

<requirements>
- Create `Extensions/AuthenticationExtensions.cs` with `ConfigureAuthentication(this WebApplicationBuilder builder)` — identical JWT bearer setup as today (same secret lookup, same `TokenValidationParameters`).
- Create `Extensions/CompressionExtensions.cs` with `ConfigureCompression(this WebApplicationBuilder builder)` — preserves Brotli + `application/json; charset=utf-8` MIME type.
- Create `Extensions/CorsExtensions.cs` with `ConfigureCors(this WebApplicationBuilder builder)` for service registration and a separate `UseDefaultCors(this WebApplication app)` that applies `AllowAnyOrigin/Method/Header` — matches today's pipeline.
- Create `Extensions/DataContextExtensions.cs` with `ConfigureDataContext(this WebApplicationBuilder builder)` — SQL Server connection string from `DefaultConnection`, identical to today.
- Create `Extensions/SwaggerExtensions.cs` with `ConfigureSwagger(this WebApplicationBuilder builder)` and `UseSwaggerUi(this WebApplication app)` — same title/version and endpoint path.
- Create `Extensions/SettingsExtensions.cs` with `LoadSettings(this WebApplication app)` — unchanged.
- Create `Extensions/ServicesExtensions.cs` (or similar) that moves the `ITokenService` / `ICryptographerService` scoped registrations introduced in task 2.0.
- Create `Extensions/OutputCacheExtensions.cs` with `ConfigureOutputCache(this WebApplicationBuilder builder)` and `UseDefaultOutputCache(this WebApplication app)` — adds the middleware needed later for anonymous lookup GETs (`Gender`, `Category`, `Difficulty`) that currently use `[ResponseCache]`. Use default policy; per-endpoint `.CacheOutput(...)` is applied in feature tasks 7.0.
- `Program.cs` after this task: contains only top-level statements of the form `builder.ConfigureX(); … var app = builder.Build(); app.UseX(); … app.MapControllers(); … app.Run();`. No inline DI calls, no inline middleware configuration beyond extension method invocations.
- The `app.MapControllers()` call MUST remain in `Program.cs` until task 13.0 deletes `Controllers/`. Auto-endpoint mapping is added in task 4.0.
- Preserve the current middleware ORDER exactly: `UseDeveloperExceptionPage` (dev only) → `UseHttpsRedirection` → `UseAuthentication` → `UseAuthorization` → `UseResponseCompression` → `UseStaticFiles (Resources)` → `UseCors` → `UseOutputCache` (new, no-op for now) → `MapControllers` → `UseSwagger` + `UseSwaggerUI`.
</requirements>

## Subtasks

- [ ] 3.1 Create each extension class in `Extensions/` and migrate the corresponding block out of `Program.cs` verbatim (change only what's needed to make it a static extension method).
- [ ] 3.2 Move the `ITokenService` / `ICryptographerService` registrations from task 2.0 into `ServicesExtensions.cs` (or similar) so `Program.cs` only calls extensions.
- [ ] 3.3 Add `ConfigureOutputCache` + `UseDefaultOutputCache` (empty default policy; middleware only).
- [ ] 3.4 Rewrite `Program.cs` as a minimal composition root.
- [ ] 3.5 Manual Verification.

## Implementation Details

See `techspec.md` → "System Architecture → Component Overview" (new `Extensions/*` classes) and "Development Sequencing → Build Order → step 1".

Do **not** add `AddEndpoints` / `AddValidators` / `AddProblemDetails` / `AddExceptionHandler<T>()` in this task — those are all task 4.0.

## Success Criteria

- `Program.cs` contains no inline `builder.Services.Add*` or `app.Use*` configuration beyond extension method calls, `app = builder.Build()`, middleware invocation, `MapControllers`, and `app.Run()`.
- Every extension class compiles in isolation (no cross-extension coupling beyond what `WebApplicationBuilder`/`WebApplication` naturally provides).
- All existing routes behave identically.

## Task Tests

- [ ] Unit tests — **N/A per PRD (out of scope).**
- [ ] Integration tests — **N/A per PRD (out of scope).**
- [ ] **Manual Verification**
  - [ ] `dotnet build` succeeds.
  - [ ] `dotnet watch run` starts; Swagger UI loads at `https://localhost:5001/swagger`.
  - [ ] `GET /v1/genders` (anon), `GET /v1/bikes` (auth), `POST /v1/accounts/login` all return identical responses to pre-task state.
  - [ ] HTTPS redirect, CORS headers, and Brotli compression still work (check `Content-Encoding: br` response header on a JSON response).
  - [ ] Static file `GET /Resources/Images/<seeded image>` still serves the file.

<critical>ALWAYS CREATE AND EXECUTE TASK TESTS BEFORE CONSIDERING IT COMPLETED</critical>

## Relevant Files

- `Extensions/AuthenticationExtensions.cs` (new)
- `Extensions/CompressionExtensions.cs` (new)
- `Extensions/CorsExtensions.cs` (new)
- `Extensions/DataContextExtensions.cs` (new)
- `Extensions/SwaggerExtensions.cs` (new)
- `Extensions/SettingsExtensions.cs` (new)
- `Extensions/ServicesExtensions.cs` (new — consolidates task 2.0 registrations)
- `Extensions/OutputCacheExtensions.cs` (new)
- `Program.cs` (rewritten)
