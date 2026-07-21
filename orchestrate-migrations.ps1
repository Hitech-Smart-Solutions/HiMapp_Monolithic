param(
	[string]$StartupProject = "HiMapp_Monolithic.csproj"
)

$projects = @(
	@{ Project = "src/Modules/Execution/Execution.Infrastructure/Execution.Infrastructure.csproj"; Context = "ExecutionDbContext" },
	@{ Project = "src/Modules/Admin/Admin.Infrastructure/Admin.Infrastructure.csproj"; Context = "AdminDbContext" },
	@{ Project = "src/Modules/PM/PM.Infrastructure/PM.Infrastructure.csproj"; Context = "PMDbContext" },
	@{ Project = "src/Modules/Safety/Safety.Infrastructure/Safety.Infrastructure.csproj"; Context = "SafetyDbContext" },
	@{ Project = "src/Modules/Store/Store.Infrastructure/Store.Infrastructure.csproj"; Context = "StoreDbContext" },
	@{ Project = "src/Shared/SharedKernel/SharedKernel.csproj"; Context = "Himapp.SharedKernel.Outbox.OutboxDbContext" }
)

foreach ($p in $projects) {
	Write-Host "Running migrations for context $($p.Context) in project $($p.Project)"
	dotnet ef database update --project $p.Project --context $p.Context --startup-project $StartupProject
	if ($LASTEXITCODE -ne 0) {
		Write-Error "Migration failed for $($p.Project) context $($p.Context). Aborting."
		exit $LASTEXITCODE
	}
}

Write-Host "All migrations applied successfully."