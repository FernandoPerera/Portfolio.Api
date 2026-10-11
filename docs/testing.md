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
7. **Theory for Same Scenario:** When tests only differ in their input and expected values, merge them into a single `[Theory]` with `[InlineData]` instead of repeating `[Fact]` methods (e.g., `BeCreated` and `TrimSurroundingSpaces` become one `BeCreated(given, expected)`).
8. **No Redundant Tests:** If piece A invokes piece B, the tests of A must not cover a scenario already covered by the tests of B. A only tests its own behavior: composition and invariants across several parts (e.g., `TimelineTranslationShould` does not repeat the invalid language code case already in `LanguageCodeShould`).
9. **Builders for Valid Instances:** If a test needs a valid Value Object or Model only as setup and has to unwrap the `Either` (`.IfLeft(error => throw ...)`), create a builder in `Builders/` instead of unwrapping inside the test (e.g., `StartDateBuilder.Build("2023-01")`).
10. **No Repeated Setup Data:** When the same object or value (a list of translations, a language code) is needed more than once in a test, such as in the given and in the expected result, create it once in a local variable or `const` and reuse it instead of repeating it.
11. **Use Cases Test the Happy Path:** A Use Case composed with monadic LINQ (`.ToAsync()`) propagates the `Left` of every port and every Domain Model it calls by construction. Do not test failures that are produced, and already tested, in those pieces: a port failure coming back unchanged (e.g., `FailWhenEntriesCannotBeRetrieved`, `FailWhenTheEntryDoesNotExist`), or a validation or business rule of a Domain Model or VO (e.g., `FailWhenTheTranslationAlreadyExists`). The failure is tested where it is produced, in the adapter or the model. Test a failure in a Use Case only when the Use Case creates it, for example from its own rule or a decision that changes its outcome based on a port result (e.g., a port returns "not found" and the Use Case turns it into a creation).
12. **No Tests for `CreateUnsafe`:** `CreateUnsafe` factories wrap trusted data (already validated when it was created) and run no validation, so there is nothing of their own to test. Do not add tests for them.
13. **Assertions Inside `Match`:** Put every assertion of the outcome, including collaborator verifications (`Received`, `DidNotReceive`), inside the `Match` branch it belongs to (success or error) instead of after it.

---
## Reference Examples
Consult existing test implementations for canonical structure:
- **Domain VO Test Example:** `tests/Domain/ValueObjects/NameShould.cs`
- **Application UseCase Test Example:** `tests/Application/UseCases/ProfileCreatorShould.cs`
- **Domain Builder Pattern Example:** `tests/Domain/Builders/ProfileToBeCreatedBuilder.cs`