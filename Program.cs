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
using Himapp.Shared.Files;
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
using Microsoft.OpenApi;
using Microsoft.VisualBasic;
using System.IdentityModel.Tokens.Jwt;
using System.Reflection;
using System.Security.Claims;
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
builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("secret.json", optional: false, reloadOnChange: true);
AWSConfiguration.Initialize(builder.Configuration);

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
// Swagger JWT configuration
builder.Services.AddEndpointsApiExplorer();


builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Version = "v1",
        Title = "HIMAPP 2.0 API",
        Description = "HIMAPP Modular Monolith API"
    });

    // Fix duplicate DTO names across modules
    c.CustomSchemaIds(type => type.FullName);

    var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);

    if (File.Exists(xmlPath))
    {
        c.IncludeXmlComments(xmlPath, includeControllerXmlComments: true);
    }

    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT access token."
    });

    c.AddSecurityRequirement(document =>
        new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference("Bearer", document)] = []
        });
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

builder.Services.AddScoped<IWorkflowDbContext>(sp =>
    sp.GetRequiredService<WorkflowDbContext>());


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


if (app.Environment.IsDevelopment())
{
    app.MapGet("/dev/token", (IConfiguration configuration) =>
    {
        var jwt = configuration.GetSection("Jwt");
        var key = jwt["Key"];

        if (string.IsNullOrWhiteSpace(key))
        {
            return Results.Problem("JWT signing key is not configured.");
        }

        var issuer = jwt["Issuer"];
        var audience = jwt["Audience"];

        if (string.IsNullOrWhiteSpace(issuer) ||
            string.IsNullOrWhiteSpace(audience))
        {
            return Results.Problem(
                "JWT issuer or audience is not configured.");
        }

        var signingKey = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(key));

        var credentials = new SigningCredentials(
            signingKey,
            SecurityAlgorithms.HmacSha256);

        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "swagger-test-user"),
            new Claim(ClaimTypes.Name, "Swagger Test User")
        };

        var token = new JwtSecurityToken(
            issuer: issuer,
            audience: audience,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: credentials);

        return Results.Ok(new
        {
            access_token = new JwtSecurityTokenHandler()
                .WriteToken(token),
            token_type = "Bearer",
            expires_in = 900
        });
    })
    .WithTags("Development");
}



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
