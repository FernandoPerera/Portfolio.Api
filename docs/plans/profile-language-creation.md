# Feature Plan: Profile Creation with Language Validation and Multi-Language Translation Entities

### 1. Summary & Scope
- **Objective & Value:**
  Permitir registrar el idioma (`LanguageCode` de 2 letras, ej: `'es'`, `'en'`) durante la creación de un perfil (`Profiles/Create`). La persistencia se rediseña a nivel relacional para soportar internacionalización mediante la separación entre `ProfileEntity` y sus traducciones asociadas `ProfileTranslationEntity` vinculadas a `LanguageEntity`. Además, la verificación de existencia del idioma se realiza a través de un puerto de consulta de dominio (`ILanguageFinderRepository`) que recibe el Value Object `LanguageCode`.
- **Acceptance Criteria:**
  - El endpoint de creación de perfiles recibe el campo `languageCode` (o `language`).
  - El valor del idioma no puede ser vacío ni contener únicamente espacios en blanco.
  - El valor del idioma debe tener exactamente 2 letras alfabéticas (`ISO 639-1`).
  - El idioma debe existir previamente en la base de datos (`Languages`), retornando `NotFoundException` en caso contrario.
  - El puerto de búsqueda `ILanguageFinderRepository` recibe estrictamente el Value Object `LanguageCode` como parámetro de entrada (`Find(LanguageCode languageCode)`).
  - En caso de fallo en base de datos al buscar o guardar, se captura y retorna `Error.New(new ServiceUnavailableException("..."))`.
  - La persistencia guarda el perfil en `Profiles` y su contenido traducido (`Presentation`, `Role`) en `ProfileTranslations` con su FK hacia `Languages`.
  - Cada componente nuevo o modificado incluye tests unitarios o de integración siguiendo convenciones (`xUnit`, `Shouldly`, `NSubstitute`, sin comentarios `// Given...`).
- **In / Out of Scope:**
  - **In Scope:** Value Object `LanguageCode` compartido (`Profiles/Shared/Domain`), puerto `ILanguageFinderRepository`, adaptador `EntityFrameworkLanguageFinderAdapter`, entidades `LanguageEntity`, `ProfileEntity`, `ProfileTranslationEntity`, configuración en `AppDbContext`, caso de uso `ProfileCreator`, `CreateProfileRequest`, `ProfileCreatorInfo`, `ProfileCreatorController`, y adaptación de tests.
  - **Out of Scope:** Sembrado automático masivo de idiomas en migraciones iniciales, traducción dinámica automática de contenidos, modificación de versiones de .NET o paquetes NuGet.

---

### 2. Context & Architectural Impact
- **Tactical DDD & Clean Architecture:**
  - `LanguageCode` se modela como un Value Object inmutable en `Profiles/Shared/Domain/Models/LanguageCode.cs` reutilizable transversalmente.
  - El puerto de consulta `ILanguageFinderRepository` cumple la regla arquitectural de recibir un modelo de dominio (`LanguageCode`) en lugar de tipos primitivos:
    ```csharp
    public interface ILanguageFinderRepository
    {
        Task<Either<Error, Unit>> Find(LanguageCode languageCode);
    }
    ```
  - **Estructura de Entidades de Persistencia:**
    - `ProfileEntity` (Tabla `Profiles`): almacena `Id`, `Name`, `Enterprise` y la colección `Translations`.
    - `ProfileTranslationEntity` (Tabla `ProfileTranslations`): clave compuesta `(ProfileId, LanguageCode)`, contiene `Presentation`, `Role` y referencias de navegación a `Profile` y `Language`.
    - `LanguageEntity` (Tabla `Languages`): clave primaria `LanguageCode` (string 2 chars) y `Name` (string max 20).
  - El caso de uso `ProfileCreator` orquesta monádicamente (`LanguageExt`) la instanciación de `ProfileToBeCreated`, la comprobación de existencia del idioma mediante `languageFinder.Find(profile.GetLanguageCodeModel())` y la persistencia en repositorio.

---

### 3. Execution Steps (1 Step = 1 Commit)

#### Step 1: Create Shared LanguageCode Value Object & Tests
- **Objective:** Implementar el Value Object `LanguageCode` en `Profiles/Shared/Domain/Models/` con sus reglas de validación (no vacío, exactamente 2 letras alfabéticas ISO 639-1) y tests unitarios.
- **Domain & Models:**
  - `Profiles/Shared/Domain/Models/LanguageCode.cs`:
    - Constructor privado.
    - Factoría `public static Either<Error, LanguageCode> Create(string languageCode)`.
    - Validación: no nulo ni whitespace (`ValidationException("Language code cannot be empty")`), longitud exacta de 2 caracteres alfabéticos (`ValidationException("Language code must have 2 letters")`), normalización a minúsculas.
    - Método `ToString()`.
- **Target Files:**
  - `[NEW]` `Portfolio.Api/Profiles/Shared/Domain/Models/LanguageCode.cs`
  - `[NEW]` `Portfolio.Api.Tests/Profiles/Shared/Domain/Models/LanguageCodeShould.cs`
- **Tests:**
  - `BeCreated` con códigos válidos (`"es"`, `"en"`, `"fr"`).
  - `NotAllowEmptyValue` con strings vacíos o espacios (`""`, `"   "`).
  - `NotAllowInvalidLength` con longitudes distintas a 2 (`"e"`, `"esp"`).
  - `NotAllowNonAlphabeticCharacters` con caracteres numéricos o especiales (`"12"`, `"e1"`, `"e#"`).

---

#### Step 2: Create Language Persistence Infrastructure & Finder Adapter
- **Objective:** Crear la entidad `LanguageEntity`, el puerto de dominio `ILanguageFinderRepository` recibiendo `LanguageCode`, su adaptador `EntityFrameworkLanguageFinderAdapter`, registrarlo en `AppDbContext` e inyección de dependencias, y añadir tests de integración.
- **Domain & Persistence:**
  - `LanguageEntity`:
    ```csharp
    [Table("Languages")]
    public class LanguageEntity
    {
        [Key] [MaxLength(2)] 
        public string LanguageCode { get; set; } = string.Empty;

        [MaxLength(20)] 
        public string Name { get; set; } = string.Empty;
    }
    ```
  - Puerto `ILanguageFinderRepository`:
    ```csharp
    public interface ILanguageFinderRepository
    {
        Task<Either<Error, Unit>> Find(LanguageCode languageCode);
    }
    ```
  - Adaptador `EntityFrameworkLanguageFinderAdapter`:
    - Consulta `await dbContext.Languages.AnyAsync(l => l.LanguageCode == languageCode.ToString())`.
    - Si existe: retorna `Unit.Default`.
    - Si no existe: retorna `Error.New(new NotFoundException($"Language '{languageCode}' was not found"))`.
    - En caso de excepción: retorna `Error.New(new ServiceUnavailableException("Database is unavailable, error: " + error.Message))`.
  - `AppDbContext`: agregar `public DbSet<LanguageEntity> Languages { get; set; }`.
  - Inyección de dependencias: Registrar `ILanguageFinderRepository` y `EntityFrameworkLanguageFinderAdapter` en `SharedInfrastructureExtensions` o `ProfileCreateExtensions`.
- **Target Files:**
  - `[NEW]` `Portfolio.Api/Profiles/Shared/Infrastructure/Entities/LanguageEntity.cs`
  - `[NEW]` `Portfolio.Api/Profiles/Shared/Domain/Ports/ILanguageFinderRepository.cs`
  - `[NEW]` `Portfolio.Api/Profiles/Shared/Infrastructure/Adapters/EntityFrameworkLanguageFinderAdapter.cs`
  - `[MODIFY]` `Portfolio.Api/Shared/Infrastructure/Data/AppDbContext.cs`
  - `[MODIFY]` `Portfolio.Api/Shared/Infrastructure/Extensions/SharedInfrastructureExtensions.cs`
  - `[NEW]` `Portfolio.Api.Tests/Profiles/Shared/Infrastructure/Adapters/EntityFrameworkLanguageFinderAdapterShould.cs`
- **Tests:**
  - `FindLanguageWhenExists` retornando `Unit`.
  - `ReturnNotFoundWhenLanguageDoesNotExist` retornando `NotFoundException`.
  - `ReturnServiceUnavailableWhenDatabaseFails` retornando `ServiceUnavailableException`.

---

#### Step 3: Update Domain Model ProfileToBeCreated & Builder
- **Objective:** Incorporar el Value Object `LanguageCode` en el modelo `ProfileToBeCreated` y actualizar el builder de pruebas.
- **Domain & Models:**
  - `ProfileToBeCreated.cs`:
    - Incorporar campo `private readonly LanguageCode _languageCode;`.
    - Factoría `Create(string givenName, string givenPresentation, string givenJobRole, string? givenEnterpriseName, string givenLanguageCode)`.
    - Composición monádica con `from languageCode in LanguageCode.Create(givenLanguageCode)`.
    - Getters: `public string GetLanguageCode() => _languageCode.ToString();` y `public LanguageCode GetLanguageCodeModel() => _languageCode;`.
  - `ProfileToBeCreatedBuilder.cs`:
    - Parámetro por defecto `string languageCode = "es"`.
- **Target Files:**
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Domain/Models/ProfileToBeCreated.cs`
  - `[MODIFY]` `Portfolio.Api.Tests/Profiles/Create/Domain/Builders/ProfileToBeCreatedBuilder.cs`
- **Tests:**
  - Validar creación exitosa del agregado con `LanguageCode`.
  - Validar propagación de error cuando `LanguageCode` es inválido.

---

#### Step 4: Update ProfileCreator Use Case & Application DTO
- **Objective:** Actualizar `ProfileCreatorInfo` y el caso de uso `ProfileCreator` inyectando `ILanguageFinderRepository` para validar la existencia del idioma antes de crear el perfil.
- **Application & Use Case:**
  - `ProfileCreatorInfo`: añadir `string LanguageCode`.
  - `ProfileCreator`:
    - Inyectar `IProfileCreatorRepository creator` y `ILanguageFinderRepository languageFinder`.
    - Flujo monádico:
      ```csharp
      public async Task<Either<Error, Unit>> Execute(ProfileCreatorInfo profileCreatorInfo)
      {
          return await (
              from profile in ProfileToBeCreated.Create(
                  profileCreatorInfo.Name,
                  profileCreatorInfo.Presentation,
                  profileCreatorInfo.JobRole,
                  profileCreatorInfo.EnterpriseName,
                  profileCreatorInfo.LanguageCode
              ).ToAsync()
              from _ in languageFinder.Find(profile.GetLanguageCodeModel()).ToAsync()
              from result in creator.CreateProfile(profile).ToAsync()
              select result
          );
      }
      ```
- **Target Files:**
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Application/Dtos/ProfileCreatorInfo.cs`
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Application/UseCases/ProfileCreator.cs`
  - `[MODIFY]` `Portfolio.Api.Tests/Profiles/Create/Application/UseCases/ProfileCreatorShould.cs`
- **Tests:**
  - `CreateProfile` (cuando idioma existe y repositorio de creación retorna Unit).
  - `FailWhenLanguageDoesNotExist` (cuando `ILanguageFinderRepository.Find` retorna `NotFoundException`).
  - `FailWhenDatabaseUnavailableOnLanguageCheck` (cuando `ILanguageFinderRepository.Find` retorna `ServiceUnavailableException`).

---

#### Step 5: Update Infrastructure (ProfileEntity, ProfileTranslationEntity, Adapter, Request & Controller)
- **Objective:** Actualizar `ProfileEntity` y crear `ProfileTranslationEntity` con la relación hacia `LanguageEntity`. Actualizar `EntityFrameworkProfileCreatorAdapter` para persistir la entidad y su traducción, actualizar `CreateProfileRequest`, `ProfileCreatorController` y los tests de integración.
- **Infrastructure & Entities:**
  - `ProfileEntity`:
    ```csharp
    [Table("Profiles")]
    public class ProfileEntity
    {
        [Key]
        public Guid Id { get; set; }

        [Required] [MaxLength(100)] 
        public string Name { get; set; } = string.Empty;

        [MaxLength(100)] 
        public string Enterprise { get; set; } = string.Empty;

        public ICollection<ProfileTranslationEntity> Translations { get; set; } = new List<ProfileTranslationEntity>();
    }
    ```
  - `ProfileTranslationEntity`:
    ```csharp
    [Table("ProfileTranslations")]
    [PrimaryKey(nameof(ProfileId), nameof(LanguageCode))]
    public class ProfileTranslationEntity
    {
        public Guid ProfileId { get; set; }
        public ProfileEntity Profile { get; set; } = null!;

        [MaxLength(2)] 
        public string LanguageCode { get; set; } = string.Empty;

        public LanguageEntity Language { get; set; } = null!;

        [MaxLength(750)] 
        public string Presentation { get; set; } = string.Empty;

        [MaxLength(50)] 
        public string Role { get; set; } = string.Empty;
    }
    ```
  - `AppDbContext`: agregar `public DbSet<ProfileTranslationEntity> ProfileTranslations { get; set; }`.
  - `EntityFrameworkProfileCreatorAdapter`:
    ```csharp
    var profileId = generatorIdentifier.Generate();
    var profileEntity = new ProfileEntity
    {
        Id = profileId,
        Name = profileToBeCreated.GetName(),
        Enterprise = profileToBeCreated.GetEnterprise(),
        Translations = new List<ProfileTranslationEntity>
        {
            new()
            {
                ProfileId = profileId,
                LanguageCode = profileToBeCreated.GetLanguageCode(),
                Presentation = profileToBeCreated.GetPresentation(),
                Role = profileToBeCreated.GetRole()
            }
        }
    };
    await dbContext.Profiles.AddAsync(profileEntity);
    await dbContext.SaveChangesAsync();
    ```
  - `CreateProfileRequest`: `public record CreateProfileRequest(string Name, string JobRole, string Presentation, string? EnterpriseName, string LanguageCode);`.
  - `ProfileCreatorController`: mapea `request.LanguageCode` hacia `ProfileCreatorInfo` y maneja `NotFoundException` (404), `ValidationException` (400) y excepciones no controladas (500 Problem).
  - `EntityFrameworkProfileFinderAdapter` (si aplica): asegurar compatibilidad de lectura con la colección `Translations`.
- **Target Files:**
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Infrastructure/Entities/ProfileEntity.cs`
  - `[NEW]` `Portfolio.Api/Profiles/Create/Infrastructure/Entities/ProfileTranslationEntity.cs`
  - `[MODIFY]` `Portfolio.Api/Shared/Infrastructure/Data/AppDbContext.cs`
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Infrastructure/Adapters/EntityFrameworkProfileCreatorAdapter.cs`
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Infrastructure/Dtos/CreateProfileRequest.cs`
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Infrastructure/Controllers/ProfileCreatorController.cs`
  - `[MODIFY]` `Portfolio.Api/Profiles/Find/Infrastructure/Adapters/EntityFrameworkProfileFinderAdapter.cs`
  - `[MODIFY]` `Portfolio.Api.Tests/Profiles/Create/Infrastructure/Adapters/EntityFrameworkProfileCreatorAdapterShould.cs`
  - `[MODIFY]` `Portfolio.Api.Tests/Profiles/Create/Infrastructure/Controllers/ProfileCreatorControllerShould.cs`
- **Tests:**
  - `CreateProfile` en `EntityFrameworkProfileCreatorAdapterShould` verificando que la entidad `ProfileEntity` y su `ProfileTranslationEntity` asociada se guardan con todos sus campos.
  - `CreateProfile` en `ProfileCreatorControllerShould` enviando payload con `LanguageCode = "es"` (sembrando previamente el idioma en el contexto en memoria).
  - Manejo de respuestas HTTP 201 Created, 400 Bad Request y 404 Not Found.

---

### 4. Risks & Final DoD
- **Risks & Assumptions:**
  - Los tests que utilicen el contexto en memoria de EF Core deben precargar los registros de idiomas (`Languages`) para que las validaciones de claves foráneas y comprobaciones de existencia sean exitosas.
- **Feature DoD Checklist:**
  - [ ] Compilación limpia del proyecto.
  - [ ] Suite completa de tests unitarios e integración pasando (`dotnet test`).
  - [ ] Sin comentarios en el código de test.
  - [ ] Cumplimiento estricto de las reglas de arquitectura, DDD táctico, Typed Functional Programming con `LanguageExt` y puertos que reciben modelos de dominio.
