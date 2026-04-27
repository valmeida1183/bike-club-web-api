# Product Requirements Document (PRD)

**Feature:** Vertical Slice Architecture Refactor
**Project:** bike-club-api (ASP.NET Core 9.0)
**Date:** 2026-04-23

## Overview

The bike-club-api is currently organized as a Data-Driven layered application (Controllers → Services → EF Core → Models). As the surface area has grown across 11 resources, cross-cutting coupling, implicit error handling via exceptions, and scattered validation have made feature-level changes harder and onboarding slower. This initiative refactors the project into a Vertical Slice Architecture using ASP.NET Core Minimal APIs, the Result pattern for explicit error handling, and FluentValidation for request validation. All existing `/v1/` routes and business behavior are preserved. Both success and error response bodies are standardized as part of the Result pattern adoption: every endpoint that produces a body returns the `Result`/`Result<T>` envelope verbatim, with the previous bare-value success payload now nested inside `Result.Value` — this is a deliberate, client-visible contract change.

This is an internal architectural change. No new product capabilities are introduced.

## Objectives

- **Cohesion**: every endpoint's request, response, handler, validator, and routing live together in one feature folder.
- **Explicit error handling**: every handler returns a `Result`/`Result<T>`; no business flow relies on thrown exceptions for control flow.
- **Lean composition root**: `Program.cs` only wires extension methods; all service/middleware configuration lives in dedicated extension classes.
- **Endpoint auto-registration**: all endpoints are discovered via reflection (or Scrutor) rather than registered by hand.
- **Route, verb, and auth preservation**: zero changes to route paths, HTTP verbs, or auth requirements. Success response status codes are preserved; success response **bodies** are wrapped in the `Result<T>` envelope (the previous bare value moves into `Result.Value`).
- **Standardized response envelope**: every endpoint that produces a response body — success or failure, handler-returned or exception-caught — serializes the same `Result`/`Result<T>` shape so clients have a single response contract to parse.
- **Measurable outcomes**:
  - 0 files under `Controllers/` after completion.
  - 0 `System.ComponentModel.DataAnnotations` validation attributes on domain entities after completion.
  - 100% of endpoints under `Features/` use Minimal API mapping and return `Result`/`Result<T>`.
  - 100% of endpoints that produce a response body serialize the `Result`/`Result<T>` envelope (no bare values, no `ProblemDetails`).
  - `Program.cs` contains no inline service or middleware configuration — only extension method calls and `app.Run()`.
  - All existing `/v1/` routes respond with identical status codes; success response **values** match today's bare payloads exactly when read from the new envelope's `Value` property (verified manually via Swagger/Postman).

## User Stories

**Primary persona — Backend developer working on the API**

- As a backend developer, I want every piece of code for an endpoint (route, request, response, validator, handler) in one folder so I can change a feature without hunting across layers.
- As a backend developer, I want handlers to return `Result` so I can see every failure path explicitly and stop relying on exceptions for control flow.
- As a backend developer, I want request validation declared in a FluentValidation validator so validation rules are testable and decoupled from entities.

**Secondary persona — New contributor onboarding**

- As a new contributor, I want to understand one feature end-to-end by opening a single folder, so I can become productive without reading the whole codebase.

**Secondary persona — Tech lead / architect**

- As a tech lead, I want `Program.cs` to be a short, declarative composition root so startup behavior is easy to audit.
- As a tech lead, I want endpoints auto-registered via assembly scanning so forgetting to wire a new feature cannot happen.

**Tertiary persona — API consumer (existing clients)**

- As an API consumer, I want the same `/v1/` routes, verbs, and success status codes after the refactor so route-level integration is preserved.
- As an API consumer, I want a single, consistent envelope across success and error responses so my client code parses one shape regardless of outcome. (Acknowledged: this is a breaking change to response bodies — the previous bare value now lives at `response.value`.)

## Main Features

### 1. Solution Restructure

Introduce four top-level folders: `Features/`, `Extensions/`, `SharedKernel/`, `Domain/`. Retire `Controllers/`, `Services/`, `Models/`, `Static/`, and root-level `Settings.cs` as their contents are migrated.

**Functional Requirements**

1.1. The project MUST contain `Features/`, `Extensions/`, `SharedKernel/`, and `Domain/` folders at the project root.
1.2. `Models/` MUST be renamed to `Entities/` and relocated under `Domain/Entities/`.
1.3. `Static/` (e.g., `RoleStatic`, `GenderStatic`) MUST be moved under `SharedKernel/`.
1.4. `Settings.cs` MUST be moved under `SharedKernel/`.
1.5. `Resources/` MAY remain at the project root if required by ASP.NET Core static-file serving; otherwise it MUST be moved under `SharedKernel/`.
1.6. After migration completes, the `Controllers/` folder MUST be deleted.
1.7. After migration completes, all `[Required]`, `[MaxLength]`, `[EmailAddress]`, and other `System.ComponentModel.DataAnnotations` validation attributes MUST be removed from entity classes.

### 2. Feature Slice Convention

Each current controller becomes a feature folder. Each public endpoint becomes an operation folder within that feature.

**Functional Requirements**

2.1. For each resource (Account, Address, Bike, Category, Difficulty, Gender, Purchase, Role, ShopCart, Tour, User) a folder MUST exist under `Features/`.
2.2. Within each feature folder, every endpoint operation MUST have its own operation folder named in the form `[Verb][Feature]` (e.g., `GetAddress`, `GetAddressById`, `CreateAddress`, `UpdateAddress`, `DeleteAddress`).
2.3. Each operation folder MUST contain (as applicable to the operation):
   - `[Operation]Request.cs` — a `record` representing the input payload (omit when the operation has no body/query input).
   - `[Operation]Response.cs` — a `record` representing the returned data shape.
   - `[Operation]Endpoint.cs` — a Minimal API endpoint class implementing `IEndpoint`.
   - `[Operation]Handler.cs` — the orchestration class invoked by the endpoint.
   - `[Operation]Validator.cs` — a FluentValidation `AbstractValidator<TRequest>` (only when the operation has a request to validate).
2.4. Logic or validators reused by more than one operation within the same feature MUST be placed in a `Shared/` folder inside that feature folder.
2.5. Logic or validators reused across multiple features MUST live in `SharedKernel/` or `Domain/Services/` as appropriate.

### 3. Minimal API Endpoints

All endpoints migrate from attribute-routed controllers to Minimal API endpoint classes.

**Functional Requirements**

3.1. Every `[Operation]Endpoint` class MUST implement an `IEndpoint` interface exposing `void MapEndpoint(IEndpointRouteBuilder app)`.
3.2. All endpoints MUST preserve their current route path, HTTP verb, and `/v1/` prefix.
3.3. All endpoints MUST preserve their current authentication and authorization requirements (JWT bearer, `[Authorize(Roles = ...)]` equivalents via `.RequireAuthorization(...)`).
3.4. Endpoints MUST delegate execution to their `[Operation]Handler` and translate the returned `Result` into an HTTP response.

### 4. Result Pattern for Explicit Error Handling and Standardized Response Envelope

Every handler returns a `Result` or `Result<T>` value. Endpoints serialize that `Result`/`Result<T>` object as the HTTP response body and pick the status code from the `Error.Type`. Exceptions remain reserved for unexpected/infrastructure failures, but those are also surfaced through the same `Result` envelope so the wire contract is uniform.

**Functional Requirements**

4.1. `SharedKernel/` MUST contain a `Result`, `Result<T>`, `Error` record, and `ValidationResult` (aggregating multiple validation errors) type.
4.2. Every handler's public method MUST return `Result` or `Result<T>`.
4.3. Endpoints MUST translate the handler's `Result` into an HTTP response by:
   - Picking the status code from the `Error.Type` for failures (validation → 400, not-found → 404, conflict → 409, unauthorized → 401, forbidden → 403; default `Failure` → 400) and `200 OK` for success — exact mapping is a Tech Spec concern.
   - Serializing the entire `Result`/`Result<T>` object as the response body for any verb that produces a body (`Ok(result)`, `BadRequest(result)`, `NotFound(result)`, `Conflict(result)`, etc.). The success payload that was previously returned as a bare value MUST now be nested in `Result.Value`.
   - Returning **no body** (`NoContent`) only when the operation's verb is conventionally body-less and the result is a non-generic `Result.Success()`. Failures still carry the Result body even when the success path returns 204.
   - For statuses that the framework's helpers do not accept a body for (`Results.Unauthorized()`, `Results.Forbid()`), the body is omitted by necessity; the status code alone signals the outcome.
4.4. Both success and error response bodies across **all** endpoints MUST share the single `Result`/`Result<T>` envelope shape (standardized response contract — no `ProblemDetails`, no bare-value success payloads). Status codes remain semantic.
4.5. The existing `ExceptionHandlerService` MUST be replaced by the Result-to-HTTP mapping plus a global exception middleware that converts unexpected exceptions into the same `Result`-envelope failure body (with an appropriate `Error.Code` / `Error.Type`) — never `ProblemDetails`.

### 5. Request Validation via FluentValidation

All request validation moves from data annotations on entities to FluentValidation validators on request records.

**Functional Requirements**

5.1. Every `[Operation]Request` that requires validation MUST have a corresponding `[Operation]Validator : AbstractValidator<TRequest>`.
5.2. Validators MUST be invoked inside the corresponding handler before any business logic runs.
5.3. Validation failures MUST be returned as a `Result.Failure` with a `ValidationResult` payload (not thrown as exceptions).
5.4. Entity classes under `Domain/Entities/` MUST NOT retain any `System.ComponentModel.DataAnnotations` validation attributes after migration.

### 6. Extension-Method Composition Root

`Program.cs` shrinks to an ordered sequence of extension-method calls. Each current setup block becomes its own extension class under `Extensions/`.

**Functional Requirements**

6.1. The `Extensions/` folder MUST contain at minimum: `ConfigureAuthentication`, `ConfigureCompression`, `ConfigureCORS`, `ConfigureDataContext`, `ConfigureSwagger`, `LoadSettings`, and `AddEndpoints`.
6.2. Each extension method MUST extend `IServiceCollection` or `WebApplicationBuilder`/`WebApplication` as appropriate to its responsibility.
6.3. `Program.cs` MUST contain no inline service registrations or middleware pipeline configuration beyond calls to the extension methods and `app.Run()`.
6.4. Extension-method call order in `Program.cs` MUST preserve the current startup semantics (e.g., authentication before authorization, CORS before routing as required).

### 7. Endpoint Auto-Registration

Endpoints are discovered and mapped automatically; adding a new feature does not require touching `Program.cs`.

**Functional Requirements**

7.1. `AddEndpoints` MUST discover all types implementing `IEndpoint` in the project assembly and register them in DI.
7.2. A companion `MapEndpoints` extension on `WebApplication` MUST resolve every registered `IEndpoint` and invoke `MapEndpoint(app)`.
7.3. Assembly scanning MAY be implemented with Scrutor or with hand-rolled reflection. The selection is a Tech Spec concern.

### 8. Incremental Migration Strategy

The refactor is delivered feature-by-feature. Controllers and Endpoints coexist during the transition; the API stays shippable at every step.

**Functional Requirements**

8.1. Foundational scaffolding (`SharedKernel/`, `Extensions/`, `Domain/`, `IEndpoint`, Result pattern, FluentValidation wiring, auto-registration, `Program.cs` shrink) MUST land before any feature is migrated.
8.2. Features MUST be migrated one at a time. Each migration MUST delete the corresponding controller only once all its endpoints have moved to `Features/[Feature]/`.
8.3. While migration is in progress, the existing Controllers and the new Endpoints MUST NOT expose duplicate routes at the same time.
8.4. The final migration step MUST delete the `Controllers/` folder, remove the obsolete `Services/ExceptionHandlerService`, and remove all data-annotation validation attributes from entities.

### 9. Cross-Cutting Services Relocation

Existing cross-cutting services move to `SharedKernel/` with their current behavior.

**Functional Requirements**

9.1. `TokenService` MUST move to `SharedKernel/` and retain its current JWT generation behavior.
9.2. `CryptographerService` MUST move to `SharedKernel/` and retain its current PBKDF2 hashing behavior.
9.3. `ExceptionHandlerService` MUST be removed; its responsibility is replaced by Result-to-HTTP mapping plus global exception middleware (see FR 4.5).

### 10. Documentation Update

`CLAUDE.md` is updated to reflect the new architecture so that future automated edits remain consistent.

**Functional Requirements**

10.1. `CLAUDE.md` MUST be updated to describe the Features/Extensions/SharedKernel/Domain layout, the Result pattern, FluentValidation usage, and endpoint auto-registration.
10.2. `CLAUDE.md` MUST no longer reference `Controllers/`, `Models/`, root-level `Services/`, `Static/`, or `Settings.cs` after the refactor completes.

## User Experience

**Developer journey — adding a new endpoint (post-refactor)**

1. Create an operation folder under the relevant `Features/[Feature]/` directory.
2. Add `Request`, `Response`, `Handler`, `Validator` (if needed), and `Endpoint` files.
3. Run the app — the endpoint is auto-registered and available. No edits to `Program.cs`, no DI wiring changes.

**Developer journey — debugging a failing endpoint**

1. Open the operation folder.
2. All relevant code (route, validation, orchestration, response mapping) is visible in one place.

**API consumer journey**

1. All existing `/v1/` routes, HTTP verbs, auth headers, and success status codes behave identically.
2. Both success and error responses now share a single, predictable `Result`/`Result<T>` envelope shape across all endpoints — clients read the previous success payload from `response.value`, and the error from `response.error` (with `response.error.type` indicating the failure category).
3. Endpoints whose verb intentionally returns no body (e.g., a delete that resolves to `204 NoContent`) keep their body-less behavior on the success path; failure paths from those same endpoints still carry the Result envelope.

## High-Level Technical Constraints

- **Platform**: ASP.NET Core 9.0, EF Core 9.0.1 (SQL Server provider), C#. Must not change framework versions as part of this refactor.
- **Authentication**: existing JWT bearer flow (`/v1/accounts/login`, `/v1/accounts/register`, `Authorization: Bearer` header, `Monitor`/`Cyclist` roles) must continue to work identically.
- **Routing**: all existing `/v1/` route paths and HTTP verbs MUST be preserved exactly. Swagger must continue to document them at `/swagger`.
- **Static files**: `Resources/Images/` are served at `/Resources`. If ASP.NET Core requires the physical folder at the project root, `Resources/` stays at the root.
- **Database**: no schema changes, no new migrations solely as a result of the refactor. Entity property names and types remain identical.
- **Response envelope**: both success and error bodies move to a single standardized `Result`/`Result<T>` envelope (the same object the handler returns). The previous bare-value success payload is now nested in `Result.Value`. `ProblemDetails` is **not** used. The `Error.Type` enum drives the HTTP status code on failures; success uses the existing semantic status codes (typically `200 OK`, or `204 NoContent` for body-less operations).
- **Coexistence window**: Controllers and Minimal API endpoints coexist during incremental migration but MUST NOT both publish the same route simultaneously.
- **Libraries to introduce**: FluentValidation, and optionally Scrutor. Final selection is a Tech Spec decision.
- **Performance**: no regression in request latency or startup time is acceptable.

## Out of Scope

- **Automated tests.** The project currently has no test suite; adding one is explicitly deferred to a future initiative. Verification is manual via Swagger/Postman.
- **Changes to business logic or business rules.** Business behavior observable to a client (the actual data, the JWT claims, the domain effects of a request) MUST be unchanged. Note: the **shape** of the success body is intentionally changed (wrapped in the `Result<T>` envelope) — that is a wire-contract change, not a business-logic change, and it is in scope.
- **Database schema changes or new EF Core migrations.** Entities keep their current columns, relationships, and constraints. Data annotations used for EF mapping (e.g., `[Key]`, `[ForeignKey]`) are preserved; only validation annotations are removed.
- **New endpoints or new product features.**
- **Changes to the authentication scheme, role model, or JWT claims.**
- **Changes to Swagger documentation beyond what results naturally from the Minimal API migration.**
- **CI/CD, deployment, logging, observability, rate limiting, or caching changes.**
- **Frontend or mobile client changes.**
- **Introduction of MediatR, CQRS libraries, or a full Clean Architecture split.** Handlers are plain classes invoked directly by endpoints.
- **Migration of `Resources/` away from the project root if ASP.NET Core static-file serving requires it there.**
