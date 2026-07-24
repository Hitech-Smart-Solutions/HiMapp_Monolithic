# Approval Workflow Implementation Plan

## Phase 1: Workflow Module - Models
- [x] Create `src/Shared/Workflow/Models/IRequiresApproval.cs` - Marker interface
- [x] Create `src/Shared/Workflow/Models/WorkflowConfiguration.cs` - Config for approval levels
- [x] Create `src/Shared/Workflow/Models/WorkflowConstants.cs` - State/action constants
- [x] Create `src/Shared/Workflow/Models/WorkflowActionRequest.cs` - Approval action DTO

## Phase 2: Workflow Module - Filter
- [x] Create `src/Shared/Workflow/Filters/RequiresApprovalAttribute.cs` - Action filter attribute

## Phase 3: Workflow Module - Controller & Enhanced Service
- [x] Create `src/Shared/Workflow/Controllers/WorkflowController.cs` - Shared approval endpoints
- [x] Update `src/Shared/Workflow/Services/IWorkflowService.cs` - Add query methods
- [x] Update `src/Shared/Workflow/Services/InMemoryWorkflowService.cs` - Enhanced implementation
- [x] Update `src/Shared/Workflow/DependencyInjection.cs` - Register filter

## Phase 4: Execution Module Integration
- [x] Update `Execution.Application.csproj` - Add Workflow reference
- [x] Update `DailyLaborModels.cs` - Implement IRequiresApproval
- [x] Update `DailyProgressModel.cs` - Implement IRequiresApproval
- [x] Update `DailyLaborsController.cs` - Add [RequiresApproval]
- [x] Update `DailyProgressController.cs` - Add [RequiresApproval]

## Phase 5: API Host
- [x] Update `Program.cs` - Register Workflow controllers assembly

