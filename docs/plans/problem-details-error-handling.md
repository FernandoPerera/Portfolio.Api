# Feature Plan: Standardized Problem Details Error Handling with `IErrorResponseMapper`

### 1. Summary & Scope
- **Objective & Value:**
  Estandarizar el mapeo y la serialización de errores de dominio (`LanguageExt.Common.Error`) a respuestas HTTP conformes al estándar RFC 7807 / RFC 9457 (`ProblemDetails`). Se introduce un puerto de infraestructura desacoplado (`IErrorResponseMapper`) y su adaptador (`ProblemDetailsMapper`) en `Shared/Infrastructure/`, eliminando la lógica repetitiva de `switch` en los controladores (empezando por `ProfileCreatorController` y aplicable a `ProfileFinderController`).
- **Acceptance Criteria:**
  - Definir la interfaz `IErrorResponseMapper` en `Portfolio.Api/Shared/Infrastructure/Ports/` con el método `ActionResult ToActionResult(ControllerBase controller, Error error)`.
  - Implementar `ProblemDetailsMapper` en `Portfolio.Api/Shared/Infrastructure/Adapters/` que mapee:
    - `NotFoundException` ➡️ HTTP 404 (Title: `"Not Found"`, Type: `"https://http.cat/404"`).
    - `ValidationException` ➡️ HTTP 400 (Title: `"Bad Request"`, Type: `"https://http.cat/400"`).
    - `ServiceUnavailableException` ➡️ HTTP 503 (Title: `"Service Unavailable"`, Type: `"https://http.cat/503"`).
    - Excepciones no controladas / desconocidas ➡️ HTTP 500 (Title: `"Internal Server Error"`, Type: `"https://http.cat/500"`).
  - Registrar `IErrorResponseMapper` y `ProblemDetailsMapper` en el contenedor de dependencias (`SharedInfrastructureExtensions`).
  - Habilitar `builder.Services.AddProblemDetails()` en `Program.cs`.
  - Refactorizar `ProfileCreatorController` inyectando `IErrorResponseMapper` por constructor primario y delegando el manejo de errores en `.Match(...)`.
  - Actualizar la documentación arquitectural (`docs/architecture.md`) reflejando el nuevo patrón para controladores.
  - Crear tests unitarios para `ProblemDetailsMapperShould` y actualizar `ProfileCreatorControllerShould`.
  - Cumplir estrictamente con las reglas de testing (`docs/testing.md`): sin comentarios en código de tests, nomenclatura PascalCase descriptiva sin guiones bajos, y aserciones limpias con `Shouldly`.
- **In / Out of Scope:**
  - **In Scope:** `IErrorResponseMapper`, `ProblemDetailsMapper`, `SharedInfrastructureExtensions`, `Program.cs`, `ProfileCreatorController`, tests asociados y actualización de `docs/architecture.md`.
  - **Out of Scope:** Modificación de use cases, modelos de dominio, o persistencia EF Core.

---

### 2. Context & Architectural Impact
- **Clean Architecture & Hexagonal Ports/Adapters:**
  - El puerto `IErrorResponseMapper` reside en la capa de infraestructura compartida (`Shared/Infrastructure/Ports/`) al ser un contrato específico para la presentación HTTP / MVC (`ActionResult`, `ControllerBase`).
  - El adaptador `ProblemDetailsMapper` en `Shared/Infrastructure/Adapters/` traduce los errores de dominio a respuestas RFC 7807 haciendo uso de las utilidades nativas de ASP.NET Core (`ControllerBase.Problem(...)`).
- **Controladores Limpios y Declarativos:**
  - Los controladores mantienen su Primary Constructor limpio inyectando el Use Case y el `IErrorResponseMapper`.
  - La composición monádica en el endpoint se simplifica a:
    ```csharp
    return await (
        from _ in creator.Execute(info).ToAsync()
        select Unit.Default
    ).Match(
        _ => CreatedAtAction(nameof(Create), request),
        error => errorResponseMapper.ToActionResult(this, error)
    );
    ```

---

### 3. Execution Steps (1 Step = 1 Commit)

#### Step 1: Create `IErrorResponseMapper` Port, `ProblemDetailsMapper` Adapter & Tests
- **Objective:** Crear la abstracción `IErrorResponseMapper`, implementar `ProblemDetailsMapper` con soporte para RFC 7807/9110 y escribir la suite completa de pruebas unitarias.
- **Domain & Persistence / Infrastructure:**
  - `Portfolio.Api/Shared/Infrastructure/Ports/IErrorResponseMapper.cs`:
    ```csharp
    using LanguageExt.Common;
    using Microsoft.AspNetCore.Mvc;

    namespace Portfolio.Api.Shared.Infrastructure.Ports;

    public interface IErrorResponseMapper
    {
        ActionResult ToActionResult(ControllerBase controller, Error error);
    }
    ```
  - `Portfolio.Api/Shared/Infrastructure/Adapters/ProblemDetailsMapper.cs`:
    - Mapeo de `NotFoundException` (404), `ValidationException` (400), `ServiceUnavailableException` (503) y default (500).
    - Extracción de status, title, type RFC y llamada a `controller.Problem(...)` configurando `detail` e `instance`.
- **Target Files:**
  - `[NEW]` `Portfolio.Api/Shared/Infrastructure/Ports/IErrorResponseMapper.cs`
  - `[NEW]` `Portfolio.Api/Shared/Infrastructure/Adapters/ProblemDetailsMapper.cs`
  - `[NEW]` `Portfolio.Api.Tests/Shared/Infrastructure/Adapters/ProblemDetailsMapperShould.cs`
- **Tests:**
  - `ReturnNotFoundProblemDetailsWhenErrorIsNotFoundException`
  - `ReturnBadRequestProblemDetailsWhenErrorIsValidationException`
  - `ReturnServiceUnavailableProblemDetailsWhenErrorIsServiceUnavailableException`
  - `ReturnInternalServerErrorProblemDetailsWhenErrorIsUnhandledException`

---

#### Step 2: Register Problem Details Services in Dependency Injection & Program.cs
- **Objective:** Registrar `IErrorResponseMapper` en `SharedInfrastructureExtensions` y habilitar `AddProblemDetails()` en `Program.cs`.
- **Infrastructure & Configuration:**
  - `Portfolio.Api/Shared/Infrastructure/Extensions/SharedInfrastructureExtensions.cs`:
    ```csharp
    services.AddSingleton<IErrorResponseMapper, ProblemDetailsMapper>();
    ```
  - `Portfolio.Api/Program.cs`:
    - Añadir `builder.Services.AddProblemDetails();`.
- **Target Files:**
  - `[MODIFY]` `Portfolio.Api/Shared/Infrastructure/Extensions/SharedInfrastructureExtensions.cs`
  - `[MODIFY]` `Portfolio.Api/Program.cs`
- **Tests:**
  - Verificación de compilación y pruebas existentes.

---

#### Step 3: Refactor `ProfileCreatorController` & Update Controller Tests
- **Objective:** Refactorizar `ProfileCreatorController` para inyectar `IErrorResponseMapper` y actualizar sus tests en `ProfileCreatorControllerShould`.
- **Presentation & Tests:**
  - `ProfileCreatorController.cs`:
    - Primary constructor `public class ProfileCreatorController(ProfileCreator creator, IErrorResponseMapper errorResponseMapper) : ControllerBase`.
    - Sustituir bloque switch por `error => errorResponseMapper.ToActionResult(this, error)`.
  - `ProfileCreatorControllerShould.cs`:
    - Instanciar `ProfileCreatorController` pasando `new ProblemDetailsMapper()`.
    - Actualizar aserciones para verificar el `ObjectResult` con su `StatusCode` y propiedades de `ProblemDetails`.
- **Target Files:**
  - `[MODIFY]` `Portfolio.Api/Profiles/Create/Infrastructure/Controllers/ProfileCreatorController.cs`
  - `[MODIFY]` `Portfolio.Api.Tests/Profiles/Create/Infrastructure/Controllers/ProfileCreatorControllerShould.cs`
- **Tests:**
  - `CreateProfile` (201 CreatedAtActionResult).
  - `ReturnNotFoundWhenLanguageDoesNotExist` (404 ObjectResult con ProblemDetails).
  - `ReturnBadRequestWhenLanguageCodeIsInvalid` (400 ObjectResult con ProblemDetails).

---

#### Step 4: Update Documentation
- **Objective:** Actualizar `docs/architecture.md` en la sección de Controllers para documentar el uso de `IErrorResponseMapper` y `ProblemDetailsMapper`.
- **Target Files:**
  - `[MODIFY]` `docs/architecture.md`

---

### 4. Risks & Final DoD
- **Risks & Assumptions:**
  - Al usar `controller.Problem(...)` en tests unitarios directos donde `controller.ControllerContext.HttpContext` puede ser nulo, `ProblemDetailsMapper` debe acceder con seguridad a `controller.HttpContext?.Request?.Path`.
- **Feature DoD Checklist:**
  - [ ] Compilación limpia del proyecto.
  - [ ] Suite completa de tests unitarios e integración pasando (`dotnet test`).
  - [ ] Sin comentarios en el código de test.
  - [ ] Nombres de tests en PascalCase describiendo comportamiento de negocio sin guiones bajos.
  - [ ] Cumplimiento estricto de las reglas de arquitectura y convenciones de `docs/`.
