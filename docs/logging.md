# Logging

What the API writes to its log, what it never writes, and how to change it.

---
## 1. Overview

The API uses the built-in `ILogger` and writes to the console. Nothing is stored by the API itself: the host that runs the container collects the standard output.

- **In production** every line is a JSON object (`Logging:Console:FormatterName` is `json`), so it can be searched and filtered by field.
- **In `Development`** the console uses the readable `simple` format (`appsettings.Development.json`).
- Every line carries the scopes of its request (`TraceId`, `SpanId`, `RequestPath`), so all the lines of one request can be found together.

---
## 2. What Is Logged

| Level | What | Where it comes from |
|---|---|---|
| `Information` | One line per request: method, path, status code and duration. `/health` is not logged | `LoggingExtensions` (HTTP logging) |
| `Error` | Every `5xx` answer, with the exception and its stack trace | `ProblemDetailsMapper` |
| `Warning` | Every `401` and `403` answer, with the path and the reason: reused refresh token, rejected login (it names the GitHub user), invalid login code, invalid refresh token | `ProblemDetailsMapper` |
| `Warning` | Every `429`, with the IP of the client and the path | `RateLimitingExtensions` |
| `Warning` | The expired refresh tokens could not be removed. The login or the refresh still succeeds | `EntityFrameworkExpiredRefreshTokenCleanerAdapter` |

The other `4xx` answers (validation, not found, conflict) have no line of their own, because the request line already shows their status code.

An invalid or expired **access token** is rejected by the JWT middleware before it reaches a controller, so it shows up only as a `401` in the request line.

---
## 3. What Is Never Logged

- Tokens: access tokens, refresh tokens, login codes and the OAuth `state`.
- The `Authorization` header and any other header.
- Settings and secrets.
- Request and response bodies.
- **The query string.** The GitHub callback arrives as `/v1/auth/github/callback?code=...&state=...`, and the default request line of the framework would write it. The request line is built with `AddHttpLogging` and only these fields: method, path, status code and duration.

The IP of the client is written on the `429` warnings, as a security log. Keep that in mind for the retention of the logs.

---
## 4. Server Errors and What the Client Sees

When a `5xx` happens, the client receives a generic message:

- `503`: *The service is unavailable right now. Try again later.*
- `500`: *An unexpected error occurred.*

The real cause (the message of the database or of GitHub, and the stack trace) is written to the log with the `Error` level. It is not returned in the response, so internal details do not leak. The `4xx` answers keep their own message, because it is written for the client.

To keep the cause, `ServiceUnavailableError` carries the original exception in its `Cause` property, and the mapper passes it to the logger. It is the only domain error that holds an infrastructure exception, and only to be logged.

---
## 5. Configuration

The `Logging` section of `appsettings.json` controls everything:

| Setting | Effect |
|---|---|
| `Logging:LogLevel:Default` | Minimum level of the whole application |
| `Logging:LogLevel:<Category>` | Minimum level of one category. `Microsoft.AspNetCore.HttpLogging.HttpLoggingMiddleware` is the request line: set it to `Warning` to turn it off |
| `Logging:Console:FormatterName` | `json` or `simple` |

In environment variables use `__` as separator, for example `Logging__LogLevel__Default=Warning`.
