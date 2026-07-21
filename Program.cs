using Himapp.Admin.Application;
using Himapp.Admin.Infrastructure;
using Himapp.Execution.Application;
using Himapp.Execution.Infrastructure;
using Himapp.Files;
using Himapp.Integrations.D365;
using Himapp.Notifications;
using Himapp.PM.Application;
using Himapp.PM.Infrastructure;
using Himapp.Safety.Application;
using Himapp.Safety.Infrastructure;
using Himapp.Store.Application;
using Himapp.Store.Infrastructure;
using Himapp.Workflow;
using Himapp.SharedKernel;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Himapp.SharedKernel.Logging;

var builder = WebApplication.CreateBuilder(args);

// Add shared logging configuration (console + debug). For production, replace with Serilog + OpenTelemetry.
builder.Logging.ClearProviders();
builder.Logging.AddSharedLogging(builder.Configuration);

#region 🔹 Services

// Core services
builder.Services.AddAuthorization();
builder.Services.AddControllers();
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();

// Authentication (JWT) - read values from configuration: Jwt:Issuer, Jwt:Audience, Jwt:Key
builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
    .AddJwtBearer(options =>
    {
        var jwt = builder.Configuration.GetSection("Jwt");
        var key = jwt.GetValue<string>("Key");
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = jwt.GetValue<string>("Issuer"),
            ValidAudience = jwt.GetValue<string>("Audience"),
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key ?? "replace-with-secure-key"))
        };
    });

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
    // Register EF DbContexts used by modules
    .AddDbContext<ExecutionDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))
    .AddDbContext<AdminDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))
    .AddDbContext<PMDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))
    .AddDbContext<SafetyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))
    .AddDbContext<StoreDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("Default")))

    // Register shared kernel services (Outbox, logging helpers, etc.) will be added from SharedKernel.DependencyInjection
    .AddAdminModule()
    .AddSafetyModule()
    .AddExecutionModule()
    .AddPlantMachineryModule()
    .AddStoreModule();

// Register shared kernel services (IClock, ICurrentUser, Outbox service, hosted dispatcher)
builder.Services.AddSharedKernel();

#endregion

var app = builder.Build();

// Add a logging scope for application-wide enrichment (module/application name)
var _startupLogger = app.Services.GetRequiredService<Microsoft.Extensions.Logging.ILoggerFactory>().CreateLogger("Himapp.Startup");
app.Use(async (context, next) =>
{
    using (_startupLogger.BeginScope(new System.Collections.Generic.Dictionary<string, object> { ["Application"] = "HIMAPP" }))
    {
        await next();
    }
});

#region 🔹 Middleware

app.UseHttpsRedirection();
app.UseAuthentication();
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
