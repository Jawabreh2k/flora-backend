# Flora CMS — Backend API (.NET Core 9)

> Personal portfolio project. Not affiliated with or endorsed by Flora Group; the brand name and imagery are used only to make the demo realistic.

Step 1 of the 3-part dynamic UI ecosystem:

1. **Public site** (`flora-group-website`, Next.js) — fetches theme/branding from this API and maps it to CSS variables.
2. **Admin Portal** (Next.js) — JWT-authenticated editor for the configuration.
3. **Backend** (this project) — ASP.NET Core Web API + EF Core + MS SQL Server.

## What's implemented in Step 1

| Concern | Decision |
| --- | --- |
| **Theme storage** | A single `nvarchar(max)` column (`UiConfigurations.PayloadJson`) holds the whole theme/branding document as serialized JSON. Adding new colour fields needs **no migration** — only the JSON shape widens. |
| **Singleton config** | One fixed row (`Id = 1`). Self-heals to defaults if missing. |
| **Public read** | `GET /api/ui/config` — anonymous, with `ETag` / `If-None-Match` (304) + `Cache-Control`. |
| **Admin write** | `GET` + `PUT /api/admin/ui/config` — `[Authorize(Roles="Admin")]` (JWT bearer). |
| **Auth** | `POST /api/auth/login` issues a short-lived JWT; passwords stored as BCrypt hashes. |
| **Concurrency** | `rowversion` optimistic-concurrency token + monotonic `Revision` (also the ETag). |
| **Defaults** | Seeded brand tokens mirror the real `app/globals.css` (maroon `#4A0E17`, gold `#C5A880`). |

## The Flora CMS ecosystem

```
 Admin Portal (Next.js, :3001) --JWT cookie (BFF)--> .NET API (:5253) <--SQL--> SQL Server
        |                                                 ^
        | x-revalidate-secret                             | GET /api/ui/config, /api/jobs
        v                                                 |
 Public Site (Next.js, :3000) -----------------------------+
```

| Repo | Role |
| --- | --- |
| [flora-backend](https://github.com/Jawabreh2k/flora-backend) | .NET 9 API (this repo) |
| [flora-group-website](https://github.com/Jawabreh2k/flora-group-website) | Public bilingual site, themed from the API |
| [flora-admin-portal](https://github.com/Jawabreh2k/flora-admin-portal) | Admin portal: theme, jobs, companies, inquiries, subscribers |

## Architecture

The backend is a **modular monolith** built with **Clean Architecture** and **CQRS**.
Dependencies point inward only (`Api → Infrastructure → Application → Domain`); the
Domain has no dependencies at all.

```
Flora.Cms.Domain/              # Entities + enums. No framework dependencies.
Flora.Cms.Application/         # CQRS commands/queries/handlers, ports, validators.
├── Abstractions/              #   ISender + ICommand/IQuery + handlers, IPipelineBehavior;
│                              #   ports: IAppDbContext, ITokenService, IPasswordHasher,
│                              #   IFileStorage, IDateTimeProvider, ICurrentUser.
├── Common/                    #   Pipeline behaviors (validation, logging), exceptions, models.
└── Features/                  #   Vertical slices: Auth, Jobs, Applications, UiConfig.
Flora.Cms.Infrastructure/      # EF Core DbContext (impl. IAppDbContext), configs,
│                              #   migrations, seeder, JWT/BCrypt/file-storage/clock adapters.
└── DependencyInjection.cs     #   AddInfrastructure()
Flora.Cms.Api/                 # Thin controllers (delegate to ISender), exception-handling
│                              #   middleware, CurrentUser adapter, JWT/CORS/rate-limit wiring.
└── Program.cs                 #   AddApplication() + AddInfrastructure(); migrate+seed on startup.
Flora.Cms.Application.Tests/   # Unit tests (ContentMerger).
```

### CQRS pipeline

`ISender` is a small in-process dispatcher (no MediatR dependency). Each request is an
`ICommand<T>` or `IQuery<T>` handled by exactly one `ICommandHandler`/`IQueryHandler`.
Every request flows through ordered `IPipelineBehavior`s — **logging** then
**FluentValidation** — before reaching its handler. Handlers throw intent-revealing
exceptions (`ValidationException`, `NotFoundException`, `ConflictException`) that the
API's exception-handling middleware maps to RFC-7807 problem responses (400 / 404 / 409).

> The public HTTP contract (routes, request/response shapes, status codes, ETag/304
> caching, JSON casing, enum-as-string) is unchanged from the previous version.

## Run locally

**Prerequisites:** [.NET 9 SDK](https://dotnet.microsoft.com/download) and SQL Server (the default connection string uses **LocalDB**, which ships with Visual Studio; any SQL Server works if you change the connection string).

**1. Create your local secrets file** (git-ignored):

```bash
cd Flora.Cms.Api
cp appsettings.Local.example.json appsettings.Local.json     # copy ... on Windows
```

Edit `appsettings.Local.json` and set a signing key of 32+ random characters and a bootstrap admin password:

```json
{
  "Jwt": { "SigningKey": "<a-32+-char-random-secret>" },
  "Seed": { "AdminPassword": "<bootstrap-admin-password>" }
}
```

(`dotnet user-secrets` works too. `appsettings.json` ships with these values empty and is never used for secrets.)

**2. Run** (the database is created, migrated and seeded on first start):

```bash
dotnet run --project Flora.Cms.Api --launch-profile http
```

The API listens on **http://localhost:5253** and Swagger is at **/swagger**. Sign in with username `admin` and the password you set above.

**Optional:** `Contact:*` settings (Resend or SMTP) enable email notifications for contact-form messages. Without them, messages are still stored and visible in the admin portal.

**Run the full stack:** start this API first, then [flora-group-website](https://github.com/Jawabreh2k/flora-group-website) (port 3000) and [flora-admin-portal](https://github.com/Jawabreh2k/flora-admin-portal) (port 3001).

## Notes for Step 2 / Step 3

- The public site should fetch `GET /api/ui/config`, then render the `theme` map as inline CSS
  variables on `<html>` (e.g. `style="--primary:#4A0E17"`), letting Tailwind pick up changes without
  a rebuild.
- The Admin Portal authenticates via `/api/auth/login` and edits the config through `/api/admin/ui/config`.
- CORS origins are configured under `Cors:AllowedOrigins`.
