# Testing Strategy & Conventions

---
## Overview
* **Frameworks & Tools:** `xUnit`, `Shouldly`, `NSubstitute`.
* **Class Naming Standard:** `{ClassUnderTest}Should` (e.g., `ProfileCreatorShould`, `NameShould`, `ProfileCreatorControllerShould`).

---
## Mandatory Testing Rules

1. **NO Comments in Test Code:** Do NOT write comments (`// Given`, `// When`, `// Then` or any other comments) inside test methods. Maintain clear logical structure using blank lines if necessary.
2. **Inline When & Then for Synchronous Actions:** For synchronous operations (e.g., Value Object or Domain Model creation), inline execution and assertion directly (e.g. `Name.Create(givenName).Match(...)`), avoiding intermediate variable assignments.
3. **PascalCase / CamelCase Naming:** Test method names MUST NOT contain underscores. Use descriptive PascalCase describing the scenario (e.g., `BeCreated`, `NotAllowEmptyValue`, `CreateProfileSuccessfullyWhenValidDataIsProvided`).
4. **LanguageExt Assertions:** Verify `Right` and `Left` branches using `.Match(...)` and typed extensions (`.ShouldBeUnit()`, `.ShouldBeNull()`).
5. **Naming**: Test names must describe business behavior, product intent, or direct outcomes. Avoid technical jargon, framework prefixes, or implementation details (e.g. avoid `Return...`, `Is...Exception`, `...ProblemDetails`, `Database`, `Entity`, `Repository...`).
   - Direct Actions / Outcomes: `MapToNotFound` - `MapToBadRequest` - `ApproveLoan` - `CompleteCheckout` - `RegisterNewCustomer`
   - Alternative / Edge Cases: [Reject | Prevent | Fail][Action]When[BusinessCondition] ➡️ `RejectLoanWhenCreditScoreIsLow` - `PreventCheckoutWhenCartIsEmpty` - `FailRegistrationWhenEmailIsAlreadyInUse`

### Examples
- `Domain`

```csharp
public class NameShould
{
    [Fact]
    public void BeCreated()
    {
        const string givenName = "Fernando";

        Name.Create(givenName)
            .Match(
                name => name.ToString().ShouldBe(givenName),
                error => error.ShouldBeNull()
            );
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void NotAllowEmptyValue(string givenName)
    {
        Name.Create(givenName)
            .Match(
                name => name.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("Name cannot be empty")
            );
    }

    [Fact]
    public void DoNotCreateIfTheCharacterLimitIsExceeded()
    {
        var givenName = new string('a', 120);

        Name.Create(givenName)
            .Match(
                name => name.ShouldBeNull(),
                error => error.Exception.Case.ShouldBeOfType<ValidationException>()
                    .Message.ShouldBe("The name cannot exceed 100 characters")
            );
    }
}
```

- `Application`

```csharp
public class ProfileCreatorShould
{
    private readonly IProfileCreatorRepository _repository;
    private readonly ProfileCreator _creator;

    public ProfileCreatorShould()
    {
        _repository = Substitute.For<IProfileCreatorRepository>();
        _creator = new ProfileCreator(_repository);
    }

    [Fact]
    public async Task CreateProfile()
    {
        var creatorInfo = new ProfileCreatorInfo(
            Name: "Fernando",
            JobRole: "Beer software architect",
            Presentation: "Mucho gusto :)",
            EnterpriseName: "Lean Mind"
        );
        _repository.CreateProfile(Arg.Any<ProfileToBeCreated>()).Returns(Unit.Default);

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );
    }
}
```

---
### 2. Domain Builders(Domain/Builders/*Builder.cs)

```csharp
public static class ProfileToBeCreatedBuilder
{
    public static ProfileToBeCreated Build(
        string name = "Fernando",
        string jobRole = "Software Architect",
        string presentation = "Mucho gusto :)",
        string? enterprise = "Lean Mind"
    )
    {
        return ProfileToBeCreated.Create(name, jobRole, presentation, enterprise)
            .IfLeft(error => throw new InvalidOperationException($"Error building {nameof(ProfileToBeCreated)} in tests: {error}"));
    }
}
```