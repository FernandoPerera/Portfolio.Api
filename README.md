# Personal Portfolio & Technical Blog API

---
## Overview
Backend API built with .NET 10 / C# 14 using Clean Architecture, Tactical DDD, and Functional Programming via `LanguageExt`.

---
## Stack
* **Framework:** .NET 10
* **Language:** C# 14
* **Functional Library:** LanguageExt
* **Testing:** xUnit, Shouldly, NSubstitute

---
## Developer Guidelines
* **Agent Quick Rules:** Read [`CLAUDE.md`](./CLAUDE.md)
* **Architecture Rules:** See [`docs/architecture.md`](./docs/architecture.md)
* **Testing Guidelines:** See [`docs/testing.md`](./docs/testing.md)
* **Planning Workflow:** See [`docs/planning.md`](./docs/planning.md)

---
## Quick Start

### Requirements
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* A GitHub OAuth App (for the owner login, see [`docs/authentication.md`](./docs/authentication.md))

### Configuration
`appsettings.json` only has empty keys and public URLs, so the API needs these settings before it starts. Locally, set them with [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets); in production, use environment variables with `__` as separator (`GitHub__ClientSecret`).

| Setting | Secret? | Description |
|---|---|---|
| `Jwt:SigningKey` | Yes | Key used to sign the access tokens |
| `GitHub:ClientId` | No | Client id of the GitHub OAuth App |
| `GitHub:ClientSecret` | Yes | Client secret of the GitHub OAuth App |
| `GitHub:RedirectUri` | No | Callback registered in the OAuth App (`.../v1/auth/github/callback`) |
| `Auth:FrontendRedirectUri` | No | Web address where the user lands after the login |
| `Admin:Username` | No | GitHub username of the owner |
| `Admin:GitHubUserId` | Yes | Numeric GitHub id of the owner, the only one allowed to write |
| `RateLimiting:Auth:PermitLimit`, `RateLimiting:Auth:WindowSeconds` | No | Requests allowed per client IP in the window on `v1/auth/*`. `10` per `60` s by default |
| `RateLimiting:Public:PermitLimit`, `RateLimiting:Public:WindowSeconds` | No | Same for the public `GET` endpoints. `100` per `60` s by default |
| `AllowedHosts` | No | Host the API answers to. `localhost` by default; set the domain in production |

```bash
dotnet user-secrets set "Jwt:SigningKey" "<value>" --project Portfolio.Api
dotnet user-secrets set "GitHub:ClientId" "<value>" --project Portfolio.Api
dotnet user-secrets set "GitHub:ClientSecret" "<value>" --project Portfolio.Api
dotnet user-secrets set "GitHub:RedirectUri" "http://localhost:5285/v1/auth/github/callback" --project Portfolio.Api
dotnet user-secrets set "Auth:FrontendRedirectUri" "<value>" --project Portfolio.Api
dotnet user-secrets set "Admin:Username" "<value>" --project Portfolio.Api
dotnet user-secrets set "Admin:GitHubUserId" "<value>" --project Portfolio.Api
```

### Rate limiting
Requests are limited per client IP with two fixed-window policies: `auth` for `v1/auth/*` and `public` for the anonymous `GET` endpoints. A request over the limit gets `429 Too Many Requests` as `ProblemDetails`, with a `Retry-After` header. The limits are the `RateLimiting:*` settings above.

* **Behind a proxy:** the client IP is the one the API sees. If it is deployed behind a reverse proxy or a PaaS, `ForwardedHeaders` must be configured first; otherwise every visitor shares the proxy IP and the same quota. Pending until the hosting is decided.
* **Multiple instances:** the counters live in memory, so each instance counts on its own.

### Run
```bash
dotnet run --project Portfolio.Api
```
The API listens on `http://localhost:5285` (`https://localhost:7012` with the `https` profile). Check it with `GET /health`.

### API documentation
In the `Development` environment the API serves its documentation with [Scalar](https://scalar.com) at `/scalar/v1` (for example `http://localhost:5285/scalar/v1`). It describes in business terms what each endpoint does and every response it can return. The raw OpenAPI document is at `/openapi/v1.json`. Neither is served in other environments.

To try the endpoints that only the owner can use, sign in with GitHub, exchange the login code for the tokens and paste the access token in the `Bearer` authentication of Scalar.

### Build and test
```bash
dotnet build
dotnet test
```
