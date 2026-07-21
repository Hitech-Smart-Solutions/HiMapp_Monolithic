# Database Migrations — HiMapp Monolithic

This repository uses multiple EF Core DbContexts (one per module) and a shared Outbox DbContext in the SharedKernel. To keep database schema changes coordinated, follow this recommended workflow.

Recommended strategy

- Use per-module migrations that live next to the module's infrastructure project (e.g. src/Modules/Execution/Execution.Infrastructure).
- Create migrations using `dotnet ef migrations add <Name> --project <Module.Infrastructure.csproj> --startup-project HiMapp_Monolithic.csproj --context <YourDbContext>`
- Apply migrations in a controlled, ordered manner using the included `orchestrate-migrations.ps1` script. The script invokes `dotnet ef database update` for each context in the recommended order.

Outbox notes

- OutboxDbContext is defined in SharedKernel to allow modules to enqueue reliable outbound messages in the same physical database.
- To ensure an outbox entry and domain changes are committed atomically, write the outbox record within the same EF Core transaction as your module's DbContext. If your module uses its own DbContext, use IDbContextTransaction or TransactionScope to coordinate writes, or prefer an application-level pattern where module code accepts an IOutboxService and writes outbox entries explicitly before commit.

CI/CD and production

- In CI, create and test migrations in feature branches and review the generated SQL.
- In production deployments, run `orchestrate-migrations.ps1` from the release pipeline on the database connection used by the app.
- Use non-destructive and backward compatible changes where possible (add columns, new tables). If destructive changes are required, coordinate downtime or a multi-step deployment.

Security

- Do not store DB connection strings or JWT signing keys in repo files. Use environment variables, Azure Key Vault, or other secret stores in production.

If you want, I can also:
- Add EF Core migration scaffolding commands for each module (scripts to add migration templates).
- Integrate a CI pipeline YAML snippet that runs the migration script.
