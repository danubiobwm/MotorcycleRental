# Motorcycle Rental API

Small, direct README to run the MotorcycleRental project (API + PostgreSQL + RabbitMQ) locally with Docker Compose, run migrations, and test endpoints with the included `rest.http` file.

---

## Requirements

- Docker & Docker Compose (v2) installed and running
- .NET 8 SDK (optional if using Docker only)
- (Optional) `dotnet-ef` tool if you want to run migrations locally:

```bash
dotnet tool install --global dotnet-ef
```

---

## Ports (defaults used by this project)

- API: `http://localhost:5000` (adjustable in `docker-compose.yml` ports mapping)
- PostgreSQL: `5432`
- RabbitMQ (AMQP): `5672`
- RabbitMQ Management: `15672`
- Swagger UI: `http://localhost:5000/swagger` (when API is running)

---

## Quick start (recommended: Docker Compose)

From the project root (where `docker-compose.yml` is located), run:

```bash
# bring down any previous run (optional)
docker compose down --volumes --remove-orphans

# build and start services (API, Postgres, RabbitMQ)
docker compose up -d --build
```

Wait a few seconds for containers to start. Check logs:

```bash
docker compose logs -f api
docker compose logs -f postgres
docker compose logs -f rabbitmq
```

You should see the API listening on `http://[::]:8080` or `http://[::]:80` depending on configuration inside container — host port mapping to `5000` is configured by default. Access Swagger at:

```
http://localhost:5000/
```

or

```
http://localhost:5000/swagger
```

If you changed port mapping, open the mapped host port instead.

---

## Migrations & Database (if you prefer running migrations locally)

This project uses EF Core migrations stored in the `Infra` project and the API `Api` project as startup project. Example commands from the solution root:

```bash
# create migration (if you changed model)
dotnet ef migrations add <Name> --project Infra --startup-project Api

# apply migrations to DB (uses Api as startup project to pick connection string)
dotnet ef database update --project Infra --startup-project Api
```

If you run migrations **inside Docker**, the `Program.cs` is configured to call `context.Database.Migrate()` at startup and a seeder (`DbInitializer`) populates initial data automatically. If you run inside Docker Compose, ensure Postgres and RabbitMQ are healthy before API starts; `depends_on` with healthcheck is present in `docker-compose.yml` to help sequence startup.

---

## Seeded data (IDs for testing)

The project seeds some initial records (GUIDs are fixed in the seed). Example seeded identifiers (used in `rest.http`):

- Couriers:
  - João: `33333333-3333-3333-3333-333333333333`
  - Maria: `44444444-4444-4444-4444-444444444444`
- Motorcycles:
  - Honda: `11111111-1111-1111-1111-111111111111`
  - Yamaha: `22222222-2222-2222-2222-222222222222`

Use these values to quickly create rentals using the provided `rest.http` file.

---

## Endpoints (high level)

- `GET /api/Couriers` — list couriers
- `GET /api/Couriers/{id}` — get courier by id
- `POST /api/Couriers` — create courier (JSON body)
- `GET /api/Motorcycles` — list motorcycles
- `GET /api/Motorcycles/{id}` — get motorcycle by id
- `POST /api/Motorcycles` — create motorcycle
- `GET /api/Rentals` — list rentals
- `GET /api/Rentals/{id}` — get rental by id
- `POST /api/Rentals` — create rental (JSON body)
- `PATCH /api/Rentals/{id}/return` — register return (JSON body with `returnDate`)

Refer to `rest.http` for example requests ready to run from VS Code REST Client or similar tools. Swagger UI also documents the API shapes and payloads when the API runs.

---

## Common Troubleshooting

### API not reachable on `http://localhost:5000`
- Check `docker compose ps` for the `api` container ports mapping.
- Look at `docker compose logs -f api`. If the app listens on `http://[::]:8080` you may need to change the `ports` mapping (e.g. `5000:8080`) or set `ASPNETCORE_URLS=http://+:80` in `docker-compose.yml` for the `api` service.
- Ensure no local firewall or proxy blocks the port.

### EF / Migrations errors (model changes each build)
- Avoid `Guid.NewGuid()` or `DateTime.Now` inside `OnModelCreating().HasData()`; use fixed GUIDs or move seed into a runtime seeder (`DbInitializer`), which this project uses.
- Use `context.Database.Migrate()` instead of `EnsureCreated()` when using migrations.

### PostgreSQL connection errors (`host 'postgres' not found`)
- If running locally without Docker, set `Host=localhost` in the connection string or run Postgres in Docker using the included `docker-compose.yml`.
- If running with Docker Compose, the service name `postgres` is resolvable inside the Docker network only (the API container can reach `postgres` by that name).

### RabbitMQ connection issues
- Ensure RabbitMQ is healthy (`docker compose logs rabbitmq`) and the management UI on port `15672` shows the service up. The default credentials are `guest` / `guest` in the compose file.

---

## Running tests (if present)
There is a `Tests` project included. Run unit tests with:

```bash
dotnet test
```

(From solution root or specify the project path.)

---

## Useful commands summary

```bash
# build & run locally (without docker)
dotnet build
dotnet run --project Api

# run with docker compose (recommended)
docker compose up -d --build
docker compose logs -f api

# run EF migrations from root (specify projects)
dotnet ef database update --project Infra --startup-project Api

# view the API swagger on host
# http://localhost:5000/swagger
```

---

## Where to find files

- `Api/` — ASP.NET Core startup, controllers, Program.cs
- `Infra/` — EF Core DbContext, repositories, migrations, storage, messaging
- `Application/` — services and DTOs
- `Domain/` — entities & interfaces
- `Tests/` — unit/integration tests (if implemented)
- `rest.http` — pre-built HTTP requests for manual testing

---
