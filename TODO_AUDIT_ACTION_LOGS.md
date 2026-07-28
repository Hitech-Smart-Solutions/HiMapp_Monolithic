# System User Action Logs - Implementation Plan

## Phase 1: Create Shared Audit Module (`src/Shared/Audit/`)
- [ ] 1.1 Create `Audit.csproj` project file
- [ ] 1.2 Create `Models/Actions.cs` - Actions enum
- [ ] 1.3 Create `Models/TransactionActionHistory.cs` - Entity
- [ ] 1.4 Create `Abstractions/IHasProgramId.cs` - Interface
- [ ] 1.5 Create `Abstractions/IAuditService.cs` - Interface
- [ ] 1.6 Create `Services/AuditService.cs` - Channel-based implementation
- [ ] 1.7 Create `Services/BackgroundAuditConsumer.cs` - Background service
- [ ] 1.8 Create `Filters/AutoUserActionLogAttribute.cs` - Global action filter
- [ ] 1.9 Create `DependencyInjection.cs` - Service registration

## Phase 2: Update Existing Files
- [ ] 2.1 Update `HimappDbContext.cs` - Add TransactionActionHistory DbSet
- [ ] 2.2 Update `HiMapp_Monolithic.csproj` - Add project reference
- [ ] 2.3 Update `Program.cs` - Register audit services + global filter

## Phase 3: Implement IHasProgramId on DTOs
- [ ] 3.1 Admin: `LabourDto` - implement IHasProgramId
- [ ] 3.2 Execution: Various DTOs implement IHasProgramId
- [ ] 3.3 PM: `EquipmentDto` / `EquipmentRequest` - implement IHasProgramId
- [ ] 3.4 Safety: `IncidentDto` / `IncidentFormRequest` - implement IHasProgramId
- [ ] 3.5 Store: `GatePassDto` / `GatePassFormRequest` - implement IHasProgramId

## Phase 4: Build & Verify
- [ ] 4.1 Build the solution to verify compilation

