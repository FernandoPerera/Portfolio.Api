# Personal Portfolio & Technical Blog API

---
## Overview
Backend API built with .NET 10 / C# 14 using Clean Architecture, Tactical DDD, and Functional Programming via `LanguageExt`.

---
## Stack
* **Framework:** .NET 10
* **Language:** C# 14
* **Functional Library:** LanguageExt
* **Persistence:** EF Core with PostgreSQL (Npgsql)
* **Testing:** xUnit, Shouldly, NSubstitute, Testcontainers (PostgreSQL)

---
## Developer Guidelines
* **Agent Quick Rules:** Read [`CLAUDE.md`](./CLAUDE.md)
* **Architecture Rules:** See [`docs/architecture.md`](./docs/architecture.md)
* **Testing Guidelines:** See [`docs/testing.md`](./docs/testing.md)
* **Planning Workflow:** See [`docs/planning.md`](./docs/planning.md)
* **Logging:** See [`docs/logging.md`](./docs/logging.md)

---
## Quick Start

### Requirements
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [Docker](https://www.docker.com/products/docker-desktop/), for the local database and for the tests
* A GitHub OAuth App (for the owner login, see [`docs/authentication.md`](./docs/authentication.md))

### Configuration
`appsettings.json` only has empty keys and public URLs, so the API needs these settings before it starts. Locally, set them with [user secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets); in production, use environment variables with `__` as separator (`GitHub__ClientSecret`). The API validates them at startup and does not start if one is missing or invalid; `Jwt:SigningKey` needs at least 32 characters and `Admin:GitHubUserId` must be numeric.

| Setting | Secret? | Description |
|---|---|---|
| `Database:ConnectionString` | Yes | Npgsql connection string of the PostgreSQL database. `appsettings.Development.json` already points to the local one from `docker-compose.yml` |
| `Jwt:SigningKey` | Yes | Key used to sign the access tokens |
| `GitHub:ClientId` | No | Client id of the GitHub OAuth App |
| `GitHub:ClientSecret` | Yes | Client secret of the GitHub OAuth App |
| `GitHub:RedirectUri` | No | Callback registered in the OAuth App (`.../v1/auth/github/callback`) |
| `Auth:FrontendRedirectUri` | No | Web address where the user lands after the login |
| `Admin:Username` | No | GitHub username of the owner |
| `Admin:GitHubUserId` | Yes | Numeric GitHub id of the owner, the only one allowed to write |
| `Cors:AllowedOrigins` | No | Origins of the web allowed to call the API from a browser, for example `http://localhost:3000`. It is a list: in environment variables use `Cors__AllowedOrigins__0`, `Cors__AllowedOrigins__1`... If it is empty, no browser origin is allowed. Set the production web here once its domain is defined |
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

### Database
The API stores its data in PostgreSQL. The API never changes the schema: it is created by the SQL scripts in [`database/migrations`](./database/migrations), which are plain `CREATE ... IF NOT EXISTS` statements numbered in the order they must run (`001_initial_create.sql`, `002_...`).

```bash
docker compose up -d
docker compose exec -T postgres psql -U portfolio -d portfolio < database/mock-data/001_mock_data.sql
```
The first command starts PostgreSQL on `localhost:5432` (user, password and database `portfolio`) and runs the scripts of `database/migrations` when its volume is created. The second one loads sample data (languages, a profile and a timeline) and can be repeated. A script added later does not run on an existing volume: run it with `psql` the same way, or start from scratch with `docker compose down -v`.

When an entity changes, add a new numbered script to `database/migrations`; do not edit the ones already applied. In production, run the scripts against the database before deploying the API.

### Run
```bash
docker compose up -d
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
The tests that touch the database run against a real PostgreSQL started with Testcontainers, so Docker must be running. They use the scripts of `database/migrations` to create the schema.
