# Developer Portfolio & Technical Blog API

---
## Context
This backend API serves as the content engine for a personal developer portfolio and technical blog platform. It handles localized content (Spanish/English), theme synchronization, a timeline, content discovery, and transactional communications.

---
## Stack
* **Framework:** .NET 10
* **Language:** C# 14
* **Error handling:** LanguageExt

---
## Documentation & Rules
* [Architecture & Layer Guidelines](./docs/architecture.md) — Tactical DDD patterns and layer implementation guidelines.
* [Backend Conventions & Rules](./docs/backend.instructions.md) — Coding standards, C# 14 practices, and LanguageExt rules.
* [Testing Strategy](./docs/testing.md) — Testing conventions, mandatory rules, and layer-by-layer examples.

---
## Getting Started

---
### Run the Application
```bash
dotnet run --project src/Infrastructure/Infrastructure.csproj
````
---
### Run Tests
```bash
dotnet test
```