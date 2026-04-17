# Project Guidelines

## Code Style

- Follow Clean Architecture + DDD + CQRS boundaries.
- Keep `Domain` independent from infrastructure/framework concerns.
- Keep API controllers thin; orchestration belongs in Application handlers.
- Use FluentValidation for command/query validation.
- Target .NET 8 conventions used in this repo: nullable enabled, implicit usings enabled.

## Architecture

- Layers and boundaries:
  - `src/Domain`: entities, value objects, domain events, repository contracts.
  - `src/Application`: feature-based commands, queries, handlers, validators.
  - `src/Infrastructure`: EF Core persistence, repository implementations, integrations.
  - `src/API`: composition root, middleware, controllers, hubs, health checks.
- Composition and environment-specific wiring is centralized in `src/API/Program.cs`.
- Tests are split by layer in `tests/Domain.UnitTests`, `tests/Application.UnitTests`, `tests/Infrastructure.UnitTests`, and `tests/API.UnitTests`.

## Build and Test

- Restore: `dotnet restore AuraEyes_BE.sln`
- Build: `dotnet build AuraEyes_BE.sln --configuration Release --no-restore`
- Unit tests: `dotnet test AuraEyes_BE.sln --configuration Release --no-build`
- Run API locally: `dotnet run --project src/API`
- Run migrations:
  - `dotnet ef migrations add <Name> --project src/Infrastructure --startup-project src/API`
  - `dotnet ef database update --project src/Infrastructure --startup-project src/API`
- Docker deploy-style run:
  - `docker compose --env-file .env -f docker-compose.yml pull`
  - `docker compose --env-file .env -f docker-compose.yml up -d`

## Conventions

- JSON enums are serialized as strings.
- SignalR hub JWT tokens are accepted from `access_token` query for hub paths.
- In `Test` environment, fake email/payment services are wired for deterministic tests.
- Prefer feature-folder additions in `src/Application/*` over cross-cutting generic buckets.

## Docs

- Backend overview (contains legacy boilerplate sections): [../README.md](../README.md)
- Unit test guide: [../tests/README.md](../tests/README.md)
- Business flow map: [../docs/AURA_Business_Flows.md](../docs/AURA_Business_Flows.md)
- Booking implementation details: [../docs/PATIENT_BOOKING_FLOW.md](../docs/PATIENT_BOOKING_FLOW.md)
- Notification integration: [../docs/NotificationIntegrationGuide.md](../docs/NotificationIntegrationGuide.md)
- System test and test mode behavior: [../docs/SYSTEM_TEST_FLOW_AND_GUIDE.md](../docs/SYSTEM_TEST_FLOW_AND_GUIDE.md)
- Seeded accounts and quick auth testing: [../docs/TEST_ACCOUNTS.md](../docs/TEST_ACCOUNTS.md)

## Pitfalls

- `README.md` is partially boilerplate/outdated; prefer concrete commands from workflows and current solution files.
- `docker-compose.yml` expects image/env values and does not define a local `build` section.
- Repository has no root `.env.example`; ensure required env vars are provided before container runs.
- `tests/AURA.Tests.Integration` is currently not an active test project in the solution.
