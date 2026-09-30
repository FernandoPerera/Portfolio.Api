# Architecture & Design Patterns

---
## Overview
Clean Architecture coupled with Tactical Domain-Driven Design (DDD) and Functional Programming via `LanguageExt`.

---
## Layer Implementation Patterns

### 1. Domain Layer
- **Value Objects:** Immutable, `private` constructor, `public static Either<Error, T> Create(...)`.
- **Domain Models / Entities:** `private` constructor, monadic LINQ composition from Value Objects. Expose values via explicit `GetX()` getter methods for infrastructure adapters.
- **Ports:** Interfaces in `Domain/Ports/`. Accept strongly-typed Domain Models or Value Objects (never raw primitives). Return `Task<Either<Error, Unit>>` or `Task<Either<Error, TResult>>`.
- **Canonical Examples:** `src/Domain/ValueObjects/Name.cs`, `src/Domain/Models/ProfileToBeCreated.cs`, `src/Domain/Ports/IProfileCreatorRepository.cs`.

### 2. Application Layer
- **Application DTOs:** Immutable `record` with primitive types required for execution.
- **Use Cases:** Class with Primary Constructor injecting ports. Execute method returning `Task<Either<Error, Unit>>` or `Task<Either<Error, TResult>>` orchestrated via LINQ `.ToAsync()`.
- **Canonical Examples:** `src/Application/DTOs/ProfileCreatorInfo.cs`, `src/Application/UseCases/ProfileCreator.cs`.

### 3. Infrastructure Layer
- **Infrastructure DTOs:** Immutable `record` representing HTTP payloads.
- **Controllers:** Primary Constructor injecting Use Case and `IErrorResponseMapper`. `[ApiController]`, `[Route("v1/...")]`, `[Produces("application/json")]`. Functional result mapping via `.ToAsync().Match(...)`, delegating errors with `error => errorResponseMapper.ToActionResult(this, error)`. No `switch` on errors in controllers.
- **Error Mapping:** Port `IErrorResponseMapper` (`Shared/Infrastructure/Ports/`), adapter `ProblemDetailsMapper` (`Shared/Infrastructure/Adapters/`), registered in `SharedInfrastructureExtensions`. Maps domain errors to RFC 7807 `ProblemDetails` responses.
- **Adapters:** Implement domain ports. Primary Constructor injecting `DbContext` and dependencies. Wrap low-level exception throwing inside `try/catch` and map to `Error.New(new ServiceUnavailableException(...))`.
- **Canonical Examples:** `src/Infrastructure/Controllers/ProfileCreatorController.cs`, `src/Infrastructure/Adapters/EntityFrameworkProfileCreatorAdapter.cs`.