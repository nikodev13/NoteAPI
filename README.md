# NoteAPI ![example workflow](https://github.com/nikodev13/NoteAPI/actions/workflows/ci.yml/badge.svg)

Simple ASP.NET Core web API for storing notes.

### Features
- notes:
  - get by id
  - get paginated
  - create
  - update
  - delete
- authentication
- authorization
- logging

### Tech stack
- [C# 11](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/csharp-11), [.NET 7](https://dotnet.microsoft.com/en-us/), [ASP.NET Core](https://learn.microsoft.com/pl-pl/aspnet/core/introduction-to-aspnet-core?view=aspnetcore-7.0)
- [FluentValidation](https://docs.fluentvalidation.net/en/latest/)
- [Entity Framework Core 7](https://learn.microsoft.com/en-us/ef/core/)
- [Microsoft SQL Sever 2022](https://www.microsoft.com/pl-pl/sql-server/).
- [bcrypt.net](https://github.com/BcryptNet/bcrypt.net)
- [Serilog](https://serilog.net/)

### Moreover
The endpoints are registered using Minimal API.
Each endpoint is represented by three classes in single file:
the first for request, the second for registering endpoint and the last one for handling request.
Every request is validated by FluentValidation in ValidationFilter (implements IEndpointFilter).

### Swagger (API endpoints)

![note_api_swagger.png](docs%2Fnote_api_swagger.png)
