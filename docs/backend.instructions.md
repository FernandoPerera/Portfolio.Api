# Backend Rules & Coding Conventions

## Functional Flow Rules
* **Typed Functional Programming:** Error handling and flow control via `Either<Error, T>` monad.
* **No Unhandled Exceptions:** Business flow must not throw unhandled exceptions. Use `Either<Error, T>` for operations
that can fail, use `Either<Error, Unit>` for void returns.
* **Monadic Composition:** Compose asynchronous operations using LINQ comprehension (`from ... in ... select ...`)
lifted via `.ToAsync()`.
* **Error Enclosure:** Always encapsulate exceptions inside `LanguageExt.Error.New(...)`.
* **Tactical Domain-Driven Design (DDD):** Strongly typed Value Objects, immutable validation, Domain
  Models/Entities, and explicit Ports.

## C# Syntax Standards
* **Primary Constructors:** Mandatory for class dependency injection across Use Cases, Controllers, and Adapters.
* **DTO Immutability:** Application and Infrastructure DTOs must be implemented as immutable `record` types.
* **Encapsulation:** Value Objects and Domain Entities require `private` constructors and explicit static factory
methods (`Create(...)`).
* **State Access:** Domain entities must expose explicit `GetX()` getter methods for infrastructure mapping to prevent
leaking internal domain state.

## Layer Isolation Boundaries
* **Domain Layer:** Zero external framework dependencies (except `LanguageExt`). No persistence or framework attributes.
* **Application Layer:** Depends exclusively on Domain. Orchestrates ports and models via Use Cases.
* **Infrastructure Layer:** Implements Domain Ports, API Controller endpoints, DB Contexts, and External 
Adapters. Handles `try/catch` blocks for infrastructure failure translation.