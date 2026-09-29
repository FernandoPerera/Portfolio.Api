# Architecture & Design Patterns

---
## Overview
The application follows Clean Architecture coupled with Tactical Domain-Driven Design (DDD) and Functional Programming 
principles via `LanguageExt`.

---
## Layer-by-Layer Implementation Patterns

---
### 1. Domain Layer

---
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

---
#### B. Domain Models / Business Entities 
- private constructor.
- Monadic composition via LINQ (from ... in ... select ...) from constituent Value Objects.
- Explicit GetX() getter methods to access values from the infrastructure layer.

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

---
#### C. Ports
- Interfaces located in Domain/Ports/.
- Domain ports that query or search must receive a strongly-typed Domain Model or Value Object as parameter, never raw primitive types (e.g. `Find(LanguageCode languageCode)`).
- Functional and asynchronous return types: Task<Either<Error, Unit>> or Task<Either<Error, TResult>>.

```csharp
public interface IProfileCreatorRepository
{
    Task<Either<Error, Unit>> CreateProfile(ProfileToBeCreated profileToBeCreated);
}
```

---
### 2. Application Layer

---
#### A. Application DTOs
- Immutable record containing primitive types required to execute the action.

```csharp
public record ProfileCreatorInfo(
    string Name,
    string JobRole,
    string Presentation,
    string? EnterpriseName
);
```

#### B. Use Cases
- Class with a Primary Constructor injecting the required port (I...Repository).
- Method public async Task<Either<Error, Unit>> Execute(...) (or corresponding return type).
- Orchestration via monadic LINQ comprehension (ToAsync()).

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

### 3. Infrastructure Layer

---
#### A. Infrastructure DTOs (Request / Response)
- Record representing the HTTP payload.

```csharp
public record CreateProfileRequest(
    string Name,
    string JobRole,
    string Presentation,
    string? EnterpriseName
);
```
---
#### B. Controllers
- Primary Constructor injecting the Use Case.
- Attributes: [ApiController], [Route("v1/...")], [Produces("application/json")].
- Functional mapping with .ToAsync().Match<ActionResult>(Right: ..., Left: ...).

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

---
#### C. Database Entities
- Clean classes with attributes or Fluent API mapping (ProfileEntity).

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

---
#### D. Adapters
- Implement the domain port (IProfileCreatorRepository).
- Primary Constructor injecting AppDbContext and auxiliary dependencies.
- try / catch block capturing low-level exceptions and transforming them into Error.New(new ServiceUnavailableException(...)).

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
``