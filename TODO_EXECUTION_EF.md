# TODO - Execution module: DB models + APIs (Clean Architecture)

## Step 1: EF plumbing
- [x] Register `HimappDbContext` in `Program.cs` with Npgsql + `ConnectionStrings:Default`

## Step 2: Execution persistence
- [x] Add `DbSet<>` for all Execution tables in `src/Shared/Data/HimappDbContext.cs`
- [x] Add Execution EF entity classes mapped to the tables from `himapp_schema_v2.sql`
- [x] Add EF Fluent mappings (table + column names) in `HimappDbContext`



## Step 3: Execution Infrastructure (repositories)
- [ ] Create `src/Modules/Execution/Execution.Infrastructure/`

- [ ] Implement repositories for Area, Activity, ProjectActivity, RateMaster, Planning (+Details), Manpower (+Details), DPR (+Details +Photos), DailyLabor (+Details)

## Step 4: Execution Application (CQRS)
- [x] Replace Activities in-memory repository with EF repository (temporary placeholder until DTO/schema align)
- [ ] Add Commands/Queries + handlers for each aggregate

## Step 5: Execution Contracts/DTOs
- [ ] Add DTO/request/response models for each aggregate

## Step 6: Execution APIs (controllers)
- [ ] Add controllers:
  - AreasController
  - ActivitiesController
  - ProjectActivitiesController
  - RateMastersController
  - PlanningsController
  - ManpowersController
  - DprsController
  - DailyLaborsController
- [ ] Wire routes under `/v1/execution/...`

## Step 7: DependencyInjection
- [x] Update `Execution.Application/DependencyInjection.cs` to register EF repositories (Activities feature only for now)

## Step 8: Verification
- [ ] `dotnet build`
- [ ] `dotnet run` and validate Swagger endpoints

