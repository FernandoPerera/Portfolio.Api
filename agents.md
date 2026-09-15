# Assistant Guidelines and Project Rules (Portfolio.Api)

This repository is built on **.NET 10** with **C# 14** and implements a rigorous architecture oriented towards Vertical Slices and Hexagonal / Ports & Adapters, with Typed Functional Programming (LanguageExt) and Tactical DDD.

---

## 1. Fundamental and Mandatory Rule
Before generating any work plan (`/plan`), proposing design changes, writing application code, or generating tests, **you must read and strictly apply** all the guidelines, conventions, and patterns defined in:

👉 [wayOfWork.md](./wayOfWork.md)

---

## 2. Testing Standards Summary
* **No comments in test code:** Do NOT write comments in test code (no `// Given`, `// When`, `// Then` or any other comments). Structure tests clearly with blank lines if needed, but without comments.
* **Inline When and Then for synchronous actions:** For synchronous actions (such as Value Objects and Domain Models creation), inline the execution and assertion directly (e.g. `Name.Create(givenName).Match(...)`), avoiding intermediate variable assignments.
* **Test method naming in CamelCase:** Tests must be named using CamelCase / PascalCase describing the scenario without underscores (e.g., `BeCreated`, `NotAllowEmptyValue`, `ReturnValidationErrorWhenNameIsEmptyOrWhitespace`, `CreateProfileSuccessfullyWhenValidDataIsProvided`).
* **Libraries:** `xUnit`, `Shouldly`, and `NSubstitute`.
* **LanguageExt:** Verify `Right` and `Left` branches using `.Match(...)` and typed extensions.