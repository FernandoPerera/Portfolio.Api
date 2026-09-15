# Architecture Guide, Design Standards, and Workflow (Portfolio.Api)

This document defines the architecture, code conventions, design patterns, and workflow governing the development of any new feature in **Portfolio.Api**. It must be used as a mandatory reference when generating work plans (`/plan`), designing solutions, and writing code and tests.

---

## 1. Philosophy and Architectural Style

The application is built on **.NET 10** with **C# 14** and follows a combination of:
1. **Vertical Slice Architecture / Screaming Architecture (Feature-First per Action):** Code is organized around use cases and concrete business actions, not in horizontal monolithic layers.
2. **Hexagonal Architecture (Ports & Adapters) / Clean Architecture:** Total isolation of the domain from infrastructure details.
3. **Typed Functional Programming with LanguageExt:** Error handling and control flow through `Either<Error, T>`, `Option<T>`, and `Unit` monads, eliminating unhandled exceptions in the business flow.
4. **Tactical Domain-Driven Design (DDD):** Strongly typed Value Objects with immutable validation, Domain Models/Entities, and static Factory Methods.
5. **Exhaustive Testing per Layer (TDD / BDD with xUnit + Shouldly + NSubstitute):** Coverage across Value Objects, Domain Builders, Use Cases, Infrastructure Adapters, and Controllers.

---

## 2. Directory Structure

Each feature is structured by **Aggregate / Feature** and then by **Action / Use Case**:

```text
Portfolio.Api/
├── {Feature}/                          # E.g.: Profiles
│   └── {Action}/                      # E.g.: Create, Update, GetById, Delete
│       ├── Application/
│       │   ├── Dtos/                  # Application DTOs (Use case Input/Output)
│       │   │   └── {Model}{Action}Info.cs  # E.g.: ProfileCreatorInfo (record)
│       │   └── UseCases/
│       │       └── {Action}{Feature}.cs    # E.g.: ProfileCreator
│       │
│       ├── Domain/
│       │   ├── Models/                # Value Objects and Action Domain Models
│       │   │   ├── {ValueObject}.cs   # E.g.: Name, JobRole, Presentation, EnterpriseName
│       │   │   └── {DomainModel}.cs   # E.g.: ProfileToBeCreated
│       │   └── Ports/                 # Secondary interfaces (repositories, external clients)
│       │       └── I{Action}{Feature}Repository.cs # E.g.: IProfileCreatorRepository
│       │
│       └── Infrastructure/
│           ├── Controllers/           # REST API Controllers
│           │   └── {Action}{Feature}Controller.cs # E.g.: ProfileCreatorController
│           ├── Dtos/                  # HTTP Request/Response DTOs
│           │   └── {Action}{Feature}Request.cs    # E.g.: CreateProfileRequest (record)
│           ├── Adapters/              # Port implementations (EF Core, etc.)
│           │   └── EntityFramework{Action}{Feature}Adapter.cs
│           └── Entities/              # ORM Entities / DB Mapping
│               └── {Feature}Entity.cs # E.g.: ProfileEntity
│
├── Shared/                            # Cross-cutting shared elements
│   ├── Domain/
│   │   ├── Errors/                    # Domain exceptions (ValidationException, ServiceUnavailableException)
│   │   └── Ports/                     # Shared ports (IGeneratorIdentifier, etc.)
│   └── Infrastructure/
│       ├── Adapters/                  # Shared adapters (GuidGeneratorIdentifier, etc.)
│       └── Data/                      # AppDbContext and EF Core configurations
│
└── Program.cs
```

### Test Project Structure (`Portfolio.Api.Tests`)

The test project mirrors the exact structure of `Portfolio.Api`:

```text
Portfolio.Api.Tests/
├── {Feature}/
│   └── {Action}/
│       ├── Application/
│       │   └── UseCases/
│       │       └── {Action}{Feature}Should.cs
│       ├── Domain/
│       │   ├── Builders/
│       │   │   └── {DomainModel}Builder.cs
│       │   └── Models/
│       │       └── {ValueObject}Should.cs
│       └── Infrastructure/
│           ├── Adapters/
│           │   └── EntityFramework{Action}{Feature}AdapterShould.cs
│           └── Controllers/
│               └── {Action}{Feature}ControllerShould.cs
```

---

## 3. Implementation Patterns Layer by Layer

### 3.1. Domain Layer

#### A. Value Objects
- **Immutability and Encapsulation:** `private` constructor.
- **Factory Method:** `public static Either<Error, T> Create(...)`.
- **Validation with Explicit Errors:** Work with LanguageExt's `Error` wrapping a domain-specific exception (`ValidationException`).
- **Representation:** Override `ToString()` or safely expose primitive values.

```csharp
public class Name
{
    private const int CharacterLimit = 100;
    private readonly string _name;

    private Name(string name) => _name = name;

    public static Either<Error, Name> Create(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            return Error.New(new ValidationException("Name cannot be empty"));

        if (name.Length > CharacterLimit)
            return Error.New(new ValidationException("The name cannot exceed 100 characters"));

        return new Name(name);
    }

    public override string ToString() => _name;
}
```

#### B. Domain Models / Business Entities
- `private` constructor.
- Monadic composition via LINQ (`from ... in ... select ...`) from its constituent Value Objects.
- Explicit `GetX()` getter methods to access values from the infrastructure layer.

```csharp
public class ProfileToBeCreated
{
    private readonly Name _name;
    private readonly Presentation _presentation;
    private readonly JobRole _jobRole;
    private readonly EnterpriseName _enterprise;

    private ProfileToBeCreated(Name name, Presentation presentation, JobRole jobRole, EnterpriseName enterprise)
    {
        _name = name;
        _presentation = presentation;
        _jobRole = jobRole;
        _enterprise = enterprise;
    }

    public static Either<Error, ProfileToBeCreated> Create(
        string givenName,
        string givenPresentation,
        string givenJobRole,
        string? givenEnterpriseName) =>
        from name in Name.Create(givenName)
        from presentation in Presentation.Create(givenPresentation)
        from jobRole in JobRole.Create(givenJobRole)
        from enterpriseName in EnterpriseName.Create(givenEnterpriseName)
        select new ProfileToBeCreated(name, presentation, jobRole, enterpriseName);

    public string GetName() => _name.ToString();
    public string GetPresentation() => _presentation.ToString();
    public string GetRole() => _jobRole.ToString();
    public string GetEnterprise() => _enterprise.ToString();
}
```

#### C. Ports
- Interfaces in `Domain/Ports/`.
- Functional and asynchronous return types: `Task<Either<Error, Unit>>` or `Task<Either<Error, TResult>>`.

```csharp
public interface IProfileCreatorRepository
{
    Task<Either<Error, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated);
}
```

---

### 3.2. Application Layer

#### A. Application DTOs
- Immutable `record` containing primitive types required to execute the action.

```csharp
public record ProfileCreatorInfo(
    string Name,
    string JobRole,
    string Presentation,
    string? EnterpriseName
);
```

#### B. Use Cases
- Class with a Primary Constructor injecting the required port (`I...Repository`).
- Method `public async Task<Either<Error, Unit>> Execute(...)` (or corresponding return type).
- Orchestration via monadic LINQ comprehension (`ToAsync()`).

```csharp
public class ProfileCreator(IProfileCreatorRepository creator)
{
    public async Task<Either<Error, Unit>> Execute(ProfileCreatorInfo profileCreatorInfo)
    {
        return await (
            from profile in ProfileToBeCreated.Create(
                profileCreatorInfo.Name,
                profileCreatorInfo.Presentation,
                profileCreatorInfo.JobRole,
                profileCreatorInfo.EnterpriseName
            ).ToAsync()
            from _ in creator.CreateProfile(profile).ToAsync()
            select Unit.Default
        );
    }
}
```

---

### 3.3. Infrastructure Layer

#### A. Infrastructure DTOs (Request / Response)
- `record` representing the HTTP payload.

```csharp
public record CreateProfileRequest(
    string Name,
    string JobRole,
    string Presentation,
    string? EnterpriseName
);
```

#### B. Controllers
- Primary Constructor injecting the Use Case.
- Attributes: `[ApiController]`, `[Route("v1/...")]`, `[Produces("application/json")]`.
- Functional mapping with `.ToAsync().Match<ActionResult>(Right: ..., Left: ...)`.

```csharp
[ApiController]
[Route("v1/profile")]
public class ProfileCreatorController(ProfileCreator creator) : ControllerBase
{
    [HttpPost("create")]
    [Produces("application/json")]
    public async Task<ActionResult> Create(CreateProfileRequest request)
    {
        return await (
            from _ in creator.Execute(
                new ProfileCreatorInfo(request.Name, request.JobRole, request.Presentation, request.EnterpriseName)
            ).ToAsync()
            select Unit.Default
        ).Match<ActionResult>(
            _ => CreatedAtAction(nameof(Create), request),
            error => Problem()
        );
    }
}
```

#### C. Database Entities
- Clean classes with attributes or Fluent API mapping (`ProfileEntity`).

```csharp
public class ProfileEntity
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Presentation { get; set; } = string.Empty;
    public string Role { get; set; } = string.Empty;
    public string Enterprise { get; set; } = string.Empty;
}
```

#### D. Adapters
- Implement the domain port (`IProfileCreatorRepository`).
- Primary Constructor injecting `AppDbContext` and auxiliary dependencies (e.g. `IGeneratorIdentifier`).
- `try / catch` block capturing low-level exceptions and transforming them into `Error.New(new ServiceUnavailableException(...))`.

```csharp
public class EntityFrameworkProfileCreatorAdapter(
    AppDbContext dbContext,
    IGeneratorIdentifier generatorIdentifier
) : IProfileCreatorRepository
{
    public async Task<Either<Error, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated)
    {
        try
        {
            var entity = new ProfileEntity
            {
                Id = generatorIdentifier.Generate(),
                Name = profileToBeCreated.GetName(),
                Presentation = profileToBeCreated.GetPresentation(),
                Role = profileToBeCreated.GetRole(),
                Enterprise = profileToBeCreated.GetEnterprise()
            };

            await dbContext.Profiles.AddAsync(entity);
            await dbContext.SaveChangesAsync();

            return Unit.Default;
        }
        catch (Exception error)
        {
            return Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message));
        }
    }
}
```

---

## 4. Testing Guidelines and Conventions

### General Conventions
- **Test class naming:** `{ClassUnderTest}Should` (e.g., `ProfileCreatorShould`, `NameShould`, `ProfileCreatorControllerShould`).
- **Main libraries:** `xUnit`, `Shouldly`, `NSubstitute`.
- **LanguageExt assertions:** Use `.Match(...)` or typed extensions like `.ShouldBeUnit()` / `.ShouldBeNull()` on `Right` and `Left` branches.

---

### Mandatory Rule: No Comments in Test Code
Do NOT use comments (`// Given`, `// When`, `// Then` or any other comments) in test code. Structure test methods clearly with logical separation / blank lines if desired, but keep the code clean and comment-free.

---

### Mandatory Rule: Inline When and Then for Synchronous Actions
For synchronous actions (e.g. Value Objects and Domain Models creation, synchronous validation), inline the execution and assertion directly (e.g. `Name.Create(givenName).Match(...)`), avoiding intermediate variable assignments.

---

### Test Naming: CamelCase / Business Scenario-Oriented
Test names must be written in **CamelCase / PascalCase** (no underscores), describing the scenario or expected behavior in ubiquitous language:

**Recommended examples:**
* `CreateProfileSuccessfullyWhenValidDataIsProvided()`
* `ReturnValidationErrorWhenNameIsEmptyOrWhitespace(string givenName)`
* `FailWhenCharacterLimitIsExceeded()`
* `ReturnServiceUnavailableErrorWhenDatabaseFailsToSaveProfile()`
* `ReturnCreatedAtActionWhenProfileIsSuccessfullyCreated()`
* `BeCreated()`
* `NotAllowEmptyValue()`

---

### Layer-by-Layer Examples

#### 1. Value Objects (`Domain/Models/*Should.cs`)
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

#### 2. Domain Builders (`Domain/Builders/*Builder.cs`)
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

#### 3. Use Cases (`Application/UseCases/*Should.cs`)
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
    public async Task Create_profile_successfully_when_valid_data_is_provided()
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

    [Fact]
    public async Task Return_repository_error_when_profile_persistence_fails()
    {
        var creatorInfo = new ProfileCreatorInfo("Fernando", "Architect", "Bio", "Lean Mind");
        var expectedError = Error.New(new ServiceUnavailableException("Database unavailable"));
        _repository.CreateProfile(Arg.Any<ProfileToBeCreated>()).Returns(expectedError);

        var result = await _creator.Execute(creatorInfo);

        result.Match(
            unit => throw new InvalidOperationException("Should have failed"),
            error => error.ShouldBe(expectedError)
        );
    }
}
```

#### 4. Infrastructure Adapters (`Infrastructure/Adapters/*Should.cs`)
```csharp
public class EntityFrameworkProfileCreatorAdapterShould
{
    private readonly AppDbContext _dbContext;
    private readonly IGeneratorIdentifier _generatorIdentifier;
    private readonly EntityFrameworkProfileCreatorAdapter _adapter;

    public EntityFrameworkProfileCreatorAdapterShould()
    {
        var options = new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
            .Options;
        _dbContext = new AppDbContext(options);
        _generatorIdentifier = Substitute.For<IGeneratorIdentifier>();
        _adapter = new EntityFrameworkProfileCreatorAdapter(_dbContext, _generatorIdentifier);
    }

    [Fact]
    public async Task Save_profile_in_database_successfully_when_valid_domain_model_is_provided()
    {
        var generatedId = Guid.NewGuid();
        _generatorIdentifier.Generate().Returns(generatedId);
        var domainModel = ProfileToBeCreatedBuilder.Build();

        var result = await _adapter.CreateProfile(domainModel);

        result.Match(
            unit => unit.ShouldBeUnit(),
            error => error.ShouldBeNull()
        );

        var savedEntity = await _dbContext.Profiles.FindAsync(generatedId);
        savedEntity.ShouldNotBeNull();
        savedEntity.Name.ShouldBe(domainModel.GetName());
    }

    [Fact]
    public async Task Return_service_unavailable_error_when_database_fails()
    {
        var domainModel = ProfileToBeCreatedBuilder.Build();
        await _dbContext.DisposeAsync();

        var result = await _adapter.CreateProfile(domainModel);

        result.Match(
            unit => throw new InvalidOperationException("Should have failed"),
            error => error.Exception.Case.ShouldBeOfType<ServiceUnavailableException>()
        );
    }
}
```

#### 5. Controllers (`Infrastructure/Controllers/*Should.cs`)
```csharp
public class ProfileCreatorControllerShould
{
    [Fact]
    public async Task Return_created_at_action_when_profile_is_successfully_created()
    {
        var repository = Substitute.For<IProfileCreatorRepository>();
        repository.CreateProfile(Arg.Any<ProfileToBeCreated>()).Returns(Unit.Default);
        var creator = new ProfileCreator(repository);
        var controller = new ProfileCreatorController(creator);

        var request = new CreateProfileRequest("Fernando", "Architect", "Bio", "Lean Mind");

        var response = await controller.Create(request);

        var createdAtResult = response.ShouldBeOfType<CreatedAtActionResult>();
        createdAtResult.Value.ShouldBe(request);
    }
}
```

---

## 5. Work Methodology for Plans (`/plan`) and New Features

When a **Plan** or the implementation of a new feature is requested, the process must follow these steps:

### Planning and Execution Checklist:
1. **Define the Feature Slice:**
   - Aggregate name (`{Feature}`) and action name (`{Action}`).
2. **Domain Modeling (Domain Layer):**
   - Identify required Value Objects and their validation rules.
   - Define the Aggregate Domain Model / Entity with its static factory `Create` method.
   - Define the port (`Domain/Ports/I{Action}{Feature}Repository.cs`).
3. **Application Layer:**
   - Create input DTO `record {Model}{Action}Info`.
   - Create Use Case `{Action}{Feature}` with monadic orchestration (`LanguageExt`).
4. **Infrastructure Layer:**
   - Create request DTO `record {Action}{Feature}Request`.
   - Create/Update ORM Entity in `Entities` and register it in `AppDbContext`.
   - Implement adapter `EntityFramework{Action}{Feature}Adapter`.
   - Implement controller `{Action}{Feature}Controller`.
5. **Test Suite (TDD / Test Layer):**
   - Value Objects tests (`{VO}Should`).
   - Test domain builder (`{DomainModel}Builder`).
   - Use Case tests (`{UseCase}Should`).
   - Adapter tests (`{Adapter}Should`).
   - Controller tests (`{Controller}Should`).
6. **Verification and Build:**
   - Run `dotnet test` or `dotnet build` to ensure clean compilation and 100% green tests.
