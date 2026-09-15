# Assistant Guidelines and Project Rules (Portfolio.Api)

This repository implements a rigorous architecture oriented towards Vertical Slices and Hexagonal / Ports & Adapters, with Typed Functional Programming (LanguageExt) and Tactical DDD.

---

## 1. Fundamental and Mandatory Rule
Before generating any work plan (`/plan`), proposing design changes, writing application code, or generating tests, **you must read and strictly apply** all the guidelines, conventions, and patterns defined in:

👉 [wayOfWork.md](./wayOfWork.md)

---

## 2. Testing Standards Summary
* **Mandatory Given-When-Then structure:** Every test method must explicitly divide its flow with `// Given`, `// When`, `// Then` comments.
* **Business scenario-oriented naming (BDD):** Tests must be named based on the expected behavior and the business condition/scenario (e.g., `Create_profile_successfully_when_valid_data_is_provided`, `Return_validation_error_when_name_is_empty`).
* **Libraries:** `xUnit`, `Shouldly`, and `NSubstitute`.
* **LanguageExt:** Verify `Right` and `Left` branches using `.Match(...)` and typed extensions.