using Himapp.Admin.Application;
using Himapp.Execution.Application;
using Microsoft.EntityFrameworkCore;
using Himapp.Files;
using Himapp.Integrations.D365;
using Himapp.Notifications;
using Himapp.PM.Application;
using Himapp.Safety.Application;
using Himapp.Store.Application;
using Himapp.Workflow;

var builder = WebApplication.CreateBuilder(args);

#region 🔹 Services

// Core services
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();

// Swagger (VERY IMPORTANT)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Shared Services
builder.Services
    .AddHimappNotifications()
    .AddHimappWorkflow()
    .AddHimappFiles()
    .AddD365Integration();

// Modules
builder.Services
    // Register EF DbContext used by modules
    .AddDbContext<DbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))

    .AddAdminModule()
    .AddSafetyModule()
    .AddExecutionModule()
    .AddPlantMachineryModule()
    .AddStoreModule();

#endregion

var app = builder.Build();

#region 🔹 Middleware

app.UseHttpsRedirection();
app.UseAuthorization();

// Swagger UI
app.UseSwagger();
app.UseSwaggerUI();

#endregion

#region 🔹 System Endpoints

// Health check
app.MapHealthChecks("/health");

// SignalR Hub
app.MapHub<NotificationsHub>("/hubs/notifications");
app.MapControllers();

// Root redirect
app.MapGet("/", () => Results.Redirect("/swagger"));

// Architecture Info API
app.MapGet("/api/architecture", () => Results.Ok(new
{
    Name = "HIMAPP 2.0",
    Style = "Modular monolith with clean vertical slices",
    Modules = new[] { "Admin", "Safety", "Execution", "Plant & Machinery", "Store" },
    SharedServices = new[] { "Notifications", "Workflow", "Files", "D365 Integration", "Shared Kernel" }
}));

#endregion

app.Run();
