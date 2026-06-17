# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Commands

```bash
# Build the solution
dotnet build RegistroPersonas.slnx

# Run the API (from repo root)
dotnet run --project PruebaTecnica/RegistroPersonas.csproj

# Run with HTTPS profile
dotnet run --project PruebaTecnica/RegistroPersonas.csproj --launch-profile https
```

The API runs on `http://localhost:5048` (HTTP) or `https://localhost:7095` (HTTPS) in Development mode. OpenAPI docs are available at `/openapi/v1.json` in Development.

## Architecture

This is an ASP.NET Core Web API (.NET 10) using a layered Clean Architecture approach with four projects:

```
PruebaTecnica (API host)           — Controllers, DI wiring, Program.cs
RegistroUsuarios.Application       — PersonaService (business logic + validation)
RegistroUsuarios.Domain            — Persona entity, IRegistrarPersona interface
RegistroUsuarios.Infrastructure    — PersonaRepository (SQL Server via ADO.NET)
```

### Data flow

`Controller` → `PersonaService` (validates, then calls) → `PersonaRepository` (ADO.NET, SQL Server)

### Key design notes

- **No ORM**: Infrastructure uses raw ADO.NET with `SqlConnection`/`SqlCommand`.
- **Stored procedure**: Person insertion calls `sp_RegistrarPersona`; contact rows (phones, emails, addresses) are inserted directly into a `Contactos` table with `TipoContacto` byte values (1=phone, 2=email, 3=address). Both happen inside a single transaction.
- **Validation lives in `PersonaService.Validar()`**: Required fields, alphanumeric document, Latin-alphabet names, at least one email or address, and max 2 of each contact type.
- **`IRegistrarPersona` interface** (Domain) is implemented by `PersonaService`. `PersonaService` depends directly on `PersonaRepository` rather than a repository abstraction.
- **Namespace mismatch**: `PersonaService` uses namespace `RegistroPersonas.Negocio` despite living in the `RegistroUsuarios.Application` project.

## Database configuration

Connection string key is `"DB"` (in both `Program.cs` and `appsettings.json`). Default points to `(localdb)\MSSQLLocalDB`, database `Usuarios`, with Integrated Security.
