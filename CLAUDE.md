# Developer & Agent Guidelines

## Quick Commands
- Build: `dotnet build`
- Test All: `dotnet test`
- Test Single: `dotnet test --filter "FullyQualifiedName~ProfileCreatorShould"`

## Critical Architecture & Code Rules
- **Functional Core:** Control flow via `Either<Error, T>`. No unhandled exceptions. Monadic LINQ via `.ToAsync()`.
- **Domain:** Private constructors + `Create(...)` returning `Either<Error, T>`. Expose values via explicit `GetX()` methods for infrastructure mapping. Zero framework dependencies.
- **Dependency Injection:** Mandatory C# 14 Primary Constructors across Use Cases, Controllers, and Adapters.
- **DTOs:** Records for both App and Infra DTOs.

## Behavioral Restrictions for Claude CLI
- **Targeted File Reading:** Read only necessary code files.
- **Diff & Code Output:** NEVER print code diffs, patch blocks, or full file rewrites in the chat output. Assume code changes are automatically opened and reviewed in the IDE diff viewer.
- **Fail-Fast Testing:** Run `dotnet test` immediately after code changes to verify compilation and test results.
- **Notion Board:** Always set the closed user stories to Done in the Notion Board (also in the plan checklist in `docs/planning.md`).

## Detailed Guidelines (Load ONLY when required by task)
- Architectural patterns & layers: `docs/architecture.md`
- Testing conventions: `docs/testing.md` (Read ONLY when writing/updating tests)
- Workflow & Planning rules: `docs/planning.md` (Read ONLY when drafting execution plans)