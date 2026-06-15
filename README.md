# Flora CMS — Backend API (.NET Core 9)

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

## Project layout

```
Flora.Cms.Api/
├── Domain/         # Entities: UiConfiguration (JSON payload), AdminUser
├── Data/           # DbContext, EF configurations, migrations, seeder
├── Dtos/           # UiConfigPayload (Theme/Branding/Sections), Auth DTOs
├── Services/       # UiConfigService, AuthService, JwtTokenService
├── Auth/           # JwtOptions
├── Controllers/    # Public + Admin config controllers, Auth controller
└── Program.cs      # DI, EF, JWT, CORS, Swagger, migrate+seed on startup
```

## Configuration

`appsettings.json` ships with **empty** `Jwt:SigningKey`, `Seed:AdminPassword`, and a placeholder
connection string. Supply real values out-of-band — never commit secrets:

```bash
cd Flora.Cms.Api
dotnet user-secrets init
dotnet user-secrets set "Jwt:SigningKey" "<a-32+-char-random-secret>"
dotnet user-secrets set "Seed:AdminPassword" "<bootstrap-admin-password>"
dotnet user-secrets set "ConnectionStrings:Default" "Server=...;Database=FloraCms;..."
```

`appsettings.Development.json` already contains dev-only values (LocalDB + a dev signing key) so the
project runs locally with zero extra setup.

## Run

```bash
dotnet run --project Flora.Cms.Api      # migrations + seed apply automatically
```

Then open Swagger at `https://localhost:7001/swagger`, or use `Flora.Cms.Api.http`.

## Notes for Step 2 / Step 3

- The public site should fetch `GET /api/ui/config`, then render the `theme` map as inline CSS
  variables on `<html>` (e.g. `style="--primary:#4A0E17"`), letting Tailwind pick up changes without
  a rebuild.
- The Admin Portal authenticates via `/api/auth/login` and edits the config through `/api/admin/ui/config`.
- CORS origins are configured under `Cors:AllowedOrigins`.
