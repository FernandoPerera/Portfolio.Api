# Testing Strategy & Conventions

---
## Overview
- **Frameworks:** `xUnit`, `Shouldly`, `NSubstitute`.
- **Class Naming Standard:** `{ClassUnderTest}Should` (e.g., `ProfileCreatorShould`, `NameShould`).

---
## Mandatory Rules

1. **NO Comments in Test Code:**  Use blank lines for given, when and them separation.
2. **Inline Synchronous Actions:** For VO or Domain Model creation, inline execution and assertion directly without temporary variables.
3. **PascalCase Naming:** Method names MUST NOT contain underscores. Describe business behavior or outcomes (`BeCreated`, `NotAllowEmptyValue`, `ApproveLoan`).
4. **Edge / Reject Cases Naming:** Use `[Reject | Prevent | Fail][Action]When[BusinessCondition]` (e.g., `RejectLoanWhenCreditScoreIsLow`).
5. **No Technical Jargon in Test Names:** Avoid words like `Return`, `Is...Exception`, `ProblemDetails`, `Repository`, `Database`.
6. **LanguageExt Assertions:** Verify branches using `.Match(...)` or typed extensions (`.ShouldBeUnit()`, `.ShouldBeNull()`).

---
## Reference Examples
Consult existing test implementations for canonical structure:
- **Domain VO Test Example:** `tests/Domain/ValueObjects/NameShould.cs`
- **Application UseCase Test Example:** `tests/Application/UseCases/ProfileCreatorShould.cs`
- **Domain Builder Pattern Example:** `tests/Domain/Builders/ProfileToBeCreatedBuilder.cs`