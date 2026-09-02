using Amazon;
using Amazon.Extensions.NETCore.Setup;
using Amazon.S3;
using DocumentFormat.OpenXml.Office2016.Drawing.ChartDrawing;
using Himapp.Admin.Application;
// Admin.Contracts moved to Admin.Application
using Himapp.Admin.Infrastructure;
using Himapp.Api.src.Shared.Middleware;
using Himapp.Audit;
using Himapp.Execution.Application;
// Execution.Contracts moved to Execution.Application
using Himapp.Execution.Infrastructure;
using Himapp.Files;
using Himapp.Integrations.D365;
using Himapp.Notifications;
using Himapp.PM.Application;
// PM.Contracts moved to PM.Application
using Himapp.PM.Infrastructure;
using Himapp.Safety.Application;
// Safety.Contracts moved to Safety.Application
using Himapp.Safety.Infrastructure;
using Himapp.SharedKernel;
using Himapp.SharedKernel.Abstractions;
using Himapp.SharedKernel.Logging;
using Himapp.Store.Application;
// Store.Contracts moved to Store.Application
using Himapp.Store.Infrastructure;
using Himapp.Workflow.Application;
using Himapp.Workflow.Infrastructure;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add shared logging configuration (console + debug). For production, replace with Serilog + OpenTelemetry.
builder.Logging.ClearProviders();
builder.Logging.AddSharedLogging(builder.Configuration);

#region 🔹 Services

// Core services
builder.Services.AddAuthorization();
builder.Services.AddControllers()
    .AddApplicationPart(typeof(Himapp.Workflow.Application.DependencyInjection).Assembly)
    .AddAuditActionFilter().AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = null;
    })
    .AddNewtonsoftJson(options =>
        options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
    );  // 🔥 Registers the global auto-log action filter for ALL controllers
builder.Services.AddHealthChecks();
builder.Services.AddSignalR();

var awsSection = builder.Configuration.GetSection("AWS");
var accessKey = awsSection["AWS_ACCESS_KEY_ID"];
var secretKey = awsSection["AWS_SECRET_ACCESS_KEY"];
var region = RegionEndpoint.GetBySystemName(awsSection["Region"] ?? "ap-south-1");

builder.Services.AddSingleton<IAmazonS3>(sp => new AmazonS3Client(accessKey, secretKey, region));

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
builder.Services.AddSwaggerGen(options =>
{
    options.CustomSchemaIds(type => type.FullName);
});

// Shared Services
builder.Services
    .AddHimappNotifications()
    .AddWorkflowModule()
    .AddHimappFiles()
    .AddD365Integration()
    .AddAuditLogging(builder.Configuration); // 🔥 Registers audit services (DbContext, Channel, Background consumer)

// Modules
builder.Services
    .AddDbContext<ExecutionDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    // Register EF DbContexts used by modules
    .AddDbContext<AdminDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddDbContext<PMDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddDbContext<SafetyDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddDbContext<StoreDbContext>(options =>
        options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")))
    .AddWorkflowInfrastructure(builder.Configuration)

    // Register shared kernel services (Outbox, logging helpers, etc.) will be added from SharedKernel.DependencyInjection
    .AddAdminModule()
    .AddSafetyModule()
    .AddExecutionModule()
    .AddPlantMachineryModule()
    .AddStoreModule();

builder.Services.AddScoped<IExecutionDbContext>(sp =>
    sp.GetRequiredService<ExecutionDbContext>());

builder.Services.AddScoped<IAdminDbContext>(sp =>
    sp.GetRequiredService<AdminDbContext>());

builder.Services.AddScoped<IPMDbContext>(sp =>
    sp.GetRequiredService<PMDbContext>());

builder.Services.AddScoped<ISafetyDbContext>(sp =>
    sp.GetRequiredService<SafetyDbContext>());

builder.Services.AddScoped<IStoreDbContext>(sp =>
    sp.GetRequiredService<StoreDbContext>());


// Register shared kernel services (IClock, ICurrentUser, Outbox service, hosted dispatcher)
builder.Services.AddSharedKernel();

#endregion

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAllOrigins",
        builder =>
        {
            builder.AllowAnyOrigin()
                   .AllowAnyMethod()
                   .AllowAnyHeader();
        });
});

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
app.UseMiddleware<GlobalExceptionMiddleware>();
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseCors("AllowAllOrigins");
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
    SharedServices = new[] { "Notifications", "Workflow", "Files", "D365 Integration", "Shared Kernel", "Audit" }
}));

#endregion

app.Run();
