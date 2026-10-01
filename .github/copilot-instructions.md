# Copilot Instructions for EShop

Cloud-native eShop built with **.NET 10** and **.NET Aspire 13** (Minimal API backend + Blazor Server frontend). CRUD for Products, Shopping Cart, and Orders.

## Build & Run

All projects live under `src/`; the solution file is `src/EShop.sln`.

```powershell
# Build everything
dotnet build src\EShop.sln

# Build a single project
dotnet build src\EShop.ApiService\EShop.ApiService.csproj

# Run the full distributed app (AppHost orchestrates ApiService + Web via Aspire)
aspire run
```

- Prefer `aspire run` (not `dotnet run` on individual projects) to start the distributed app — it launches the Aspire dashboard and wires up service discovery between `apiservice` and `webfrontend` as defined in `src/EShop.AppHost/AppHost.cs`.
- There is no test project yet; there are no lint configs beyond the compiler's `Nullable`/`ImplicitUsings` settings enabled in every `.csproj`.
- Use the `.http` files (e.g. `EShop.ApiService.http`) to manually exercise endpoints instead of writing ad-hoc curl commands — add new request examples there when adding endpoints. `@ApiService_HostAddress` is pinned to `http://localhost:5544`; when running via `aspire run` check the dashboard for the actual assigned port if it differs.

## Project Structure

```
src/
├── EShop.sln
├── aspire.config.json           # points the Aspire CLI at EShop.AppHost
├── EShop.AppHost/                # orchestrator (not a runtime service itself)
│   └── AppHost.cs
├── EShop.ServiceDefaults/        # shared cross-cutting extensions
│   └── Extensions.cs
├── EShop.ApiService/             # Minimal API backend
│   ├── Models/                   # POCOs, one file per entity
│   ├── Services/                 # in-memory singleton services, one per entity
│   ├── Endpoints/                # MapXEndpoints() extension methods, one per entity
│   ├── Program.cs                # composition root: DI registration + endpoint mapping
│   └── EShop.ApiService.http     # manual request examples per endpoint
└── EShop.Web/                     # Blazor Server frontend
    └── Components/               # Layout/, Pages/, Routes.razor, etc.
```

Adding a new domain entity means adding one file to each of `Models/`, `Services/`, `Endpoints/`, plus registration lines in `Program.cs` and new requests in the `.http` file — not editing `Program.cs` with inline logic.

## Architecture

- **EShop.AppHost** — Aspire orchestrator. Registers each service project with `AddProject<Projects.X>(name)`, configures health checks (`WithHttpHealthCheck`), and cross-service references (`WithReference` / `WaitFor`). This is the entry point for running the whole system; it is launched via `aspire run`, not `dotnet run`.
- **EShop.ServiceDefaults** — Shared `AddServiceDefaults()` / `MapDefaultEndpoints()` extensions (`Extensions.cs`) providing OpenTelemetry, health checks (`/health`, `/alive`), and service discovery + resilient `HttpClient` defaults. Every service project (`ApiService`, `Web`) must call `builder.AddServiceDefaults()` in `Program.cs` and `app.MapDefaultEndpoints()` before mapping its own routes — don't duplicate this wiring per-project.
- **EShop.ApiService** — Minimal API backend. Domain logic is split by feature into three layers per entity:
  - `Models/*.cs` — plain POCOs (e.g. `Product`).
  - `Services/*.cs` — in-memory singleton services (e.g. `ProductService`) seeded with sample data in the constructor; expose synchronous `GetAll`/`GetById`/`Create`/`Update`/`Delete` methods operating on an in-memory `List<T>`.
  - `Endpoints/*.cs` — static classes with a `MapXEndpoints(this IEndpointRouteBuilder app)` extension method, using `app.MapGroup("/api/x")` + per-route `Map{Get,Post,Put,Delete}` calls. Route handlers take the matching service as a parameter (DI-injected) and return `Results.Ok/NotFound/Created/NoContent`.
  - Wire-up in `Program.cs`: register the service as `builder.Services.AddSingleton<XService>()` (singleton keeps the in-memory list alive for the app's lifetime) and call `app.MapXEndpoints()`.
- **EShop.Web** — Blazor Server frontend (`Components/`), calls the API via service-discovery-resolved `HttpClient` (service name `apiservice` as registered in AppHost).

## Aspire Conventions

- Always run the distributed app with `aspire run` from `src/` — it reads `aspire.config.json`, starts the AppHost, and launches the Aspire dashboard with service discovery wired between `apiservice` and `webfrontend`. Don't `dotnet run` `EShop.ApiService`/`EShop.Web` directly for normal development.
- Every new service project added to `EShop.AppHost/AppHost.cs` must be registered with `.WithHttpHealthCheck("/health")`, and dependent services must declare `.WithReference(...)` + `.WaitFor(...)` so startup ordering and service discovery stay correct.
- Always call `builder.AddServiceDefaults()` immediately after `WebApplication.CreateBuilder(args)` and `app.MapDefaultEndpoints()` before any feature endpoint mapping — this is what wires up OpenTelemetry, health checks, and resilient `HttpClient`/service-discovery defaults from `EShop.ServiceDefaults`. Never hand-roll this wiring inside a feature project.

## Best Practices & Styling

- Target framework `net10.0` with `<ImplicitUsings>enable</ImplicitUsings>` and `<Nullable>enable</Nullable>` in every `.csproj` — avoid redundant `using` directives and handle nulls explicitly (use `?`, pattern `is null`/`is not null` checks as seen in `ProductService`).
- Use C# collection expressions (`[ ... ]`) to initialize lists/arrays (e.g. seeding `List<Product>` in a service constructor), instead of `new List<T> { ... }`.
- Minimal API structure for each entity is Model → Service → Endpoints → `Program.cs` registration (see Architecture above) — always follow this layering rather than inlining route logic or business logic directly in `Program.cs`.
- Group related routes with `app.MapGroup("/api/x")` and tag them with `.WithTags("X")` rather than mapping flat top-level routes; keep the per-route lambdas thin (call into the injected service, map result to `Results.*`).
- When adding endpoints, add corresponding request examples to the project's `.http` file.
