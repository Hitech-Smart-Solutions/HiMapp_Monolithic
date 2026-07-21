# Logging & Tracing — HiMapp Monolithic

This project provides a minimal centralized logging scaffold using Microsoft.Extensions.Logging. For production-grade observability, add Serilog and OpenTelemetry.

Quick start (recommended):

1. Add Serilog packages:
   - Serilog.AspNetCore
   - Serilog.Sinks.Console
   - Serilog.Enrichers.Environment
   - Serilog.Enrichers.Thread

2. In Program.cs replace the simple logging setup with Serilog:
   - Install the Serilog packages
   - `Log.Logger = new LoggerConfiguration()...CreateLogger();`
   - `builder.Host.UseSerilog();`

3. Add OpenTelemetry for traces and metrics (optional):
   - Add `OpenTelemetry.Exporter.Console` and `OpenTelemetry.Extensions.Hosting`
   - Use `builder.Services.AddOpenTelemetryTracing(...)` and configure exporters (Jaeger/OTLP)

Enrichment

- Enrich logs with module or request context using `BeginScope` or Serilog enrichers. The app adds an application-wide scope (Application=HIMAPP) by default.

Note on secrets

- Keep connection strings and secret keys out of source control. Use environment variables or a secrets store (Key Vault).

If you'd like, I can add Serilog and OpenTelemetry package references and wire the full setup now.
