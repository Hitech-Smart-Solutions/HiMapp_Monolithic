# Task: Add Infrastructure Projects (with DbContext) for All Modules

## Steps

### ✅ Step 1: Create PM.Domain Equipment Entity
- [x] Create `PM.Domain/Equipment/Equipment.cs` entity class

### ✅ Step 2: Create Admin.Infrastructure Project
- [x] Create `Admin.Infrastructure/Admin.Infrastructure.csproj`
- [x] Create `Admin.Infrastructure/AdminDbContext.cs`
- [x] Create `Admin.Infrastructure/DependencyInjection.cs`
- [x] Create `Admin.Infrastructure/ServiceCollectionExtensions.cs`
- [x] Create `Admin.Infrastructure/Admin.Infrastructure.slnx`

### ✅ Step 3: Create PM.Infrastructure Project
- [x] Create `PM.Infrastructure/PM.Infrastructure.csproj`
- [x] Create `PM.Infrastructure/PMDbContext.cs`
- [x] Create `PM.Infrastructure/DependencyInjection.cs`
- [x] Create `PM.Infrastructure/ServiceCollectionExtensions.cs`
- [x] Create `PM.Infrastructure/PM.Infrastructure.slnx`

### ✅ Step 4: Create Safety.Infrastructure Project
- [x] Create `Safety.Infrastructure/Safety.Infrastructure.csproj`
- [x] Create `Safety.Infrastructure/SafetyDbContext.cs`
- [x] Create `Safety.Infrastructure/DependencyInjection.cs`
- [x] Create `Safety.Infrastructure/ServiceCollectionExtensions.cs`
- [x] Create `Safety.Infrastructure/Safety.Infrastructure.slnx`

### ✅ Step 5: Create Store.Infrastructure Project
- [x] Create `Store.Infrastructure/Store.Infrastructure.csproj`
- [x] Create `Store.Infrastructure/StoreDbContext.cs`
- [x] Create `Store.Infrastructure/DependencyInjection.cs`
- [x] Create `Store.Infrastructure/ServiceCollectionExtensions.cs`
- [x] Create `Store.Infrastructure/Store.Infrastructure.slnx`

### ✅ Step 6: Update Application .csproj files to reference Infrastructure
- [x] Update `Admin.Application.csproj` - add reference to `Admin.Infrastructure`
- [x] Update `PM.Application.csproj` - add reference to `PM.Infrastructure`
- [x] Update `Safety.Application.csproj` - add reference to `Safety.Infrastructure`
- [x] Update `Store.Application.csproj` - add reference to `Store.Infrastructure`

### ✅ Step 7: Update Program.cs
- [x] Add `using` statements for new Infrastructure namespaces
- [x] Register `AdminDbContext`, `PMDbContext`, `SafetyDbContext`, `StoreDbContext`
- [x] Update solution file (HiMapp_Monolithic.slnx) with new projects

### ⬜ Step 8: Build and verify
- [ ] Build the solution to verify everything compiles

