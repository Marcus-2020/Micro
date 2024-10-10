using FluentValidation;
using Micro.Core;
using Micro.Inventory.Products.CreateProduct;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;
using Serilog.Sinks.OpenTelemetry;

namespace Micro.Api.Common.Extensions;

public static class BuildExtensions
{
    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        LoadEnviromentVariables(".env");

        ApiConfiguration.ConnectionString =
            builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        Configuration.ServiceName = builder.Configuration.GetValue<string>("ServiceName") ?? "micro-api";
        Configuration.BackendUrl = builder.Configuration.GetValue<string>("Services:BackendUrl") ??
                                   Environment.GetEnvironmentVariable("BACKEND_URL") ?? string.Empty;
        Configuration.SeqUrl = builder.Configuration.GetValue<string>("Seq:Url") ??
                               Environment.GetEnvironmentVariable("SEQ_URL") ?? string.Empty;
        Configuration.SeqApiKey = builder.Configuration.GetValue<string>("Seq:ApiKey") ??
                                  Environment.GetEnvironmentVariable("SEQ_API_KEY") ?? string.Empty;
    }

    private static void LoadEnviromentVariables(string filePath)
    {
        if (!File.Exists(filePath))
            return;

        var rows = File.ReadAllLines(filePath);

        foreach (var row in rows)
        {
            var envVar = row.Split('=');
            if (envVar.Length == 2) Environment.SetEnvironmentVariable(envVar[0], envVar[1]);
        }
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x => { x.CustomSchemaIds(n => n.FullName); });
    }

    public static void AddLogger(this WebApplicationBuilder builder)
    {
        builder.Services.AddOpenTelemetry()
            .ConfigureResource(resource => resource.AddService(Configuration.ServiceName))
            .WithTracing(tracing => tracing
                .AddSource(Configuration.ServiceName)
                .AddAspNetCoreInstrumentation()
                .AddOtlpExporter(exporter =>
                {
                    exporter.Endpoint = new Uri($"{Configuration.SeqUrl}/ingest/otlp/v1/traces");
                    exporter.Protocol = OtlpExportProtocol.HttpProtobuf;
                    exporter.Headers = $"X-Seq-ApiKey={Configuration.SeqApiKey}";
                }));

        builder.Services.AddSingleton(TracerProvider.Default.GetTracer(Configuration.ServiceName));

        builder.Host.UseSerilog((_, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .WriteTo.Console()
                .WriteTo.OpenTelemetry(options =>
                {
                    options.Endpoint = $"{Configuration.SeqUrl}/ingest/otlp/v1/logs";
                    options.Headers = new Dictionary<string, string>
                    {
                        ["X-Seq-ApiKey"] = Configuration.SeqApiKey
                    };
                    options.Protocol = OtlpProtocol.HttpProtobuf;
                    options.ResourceAttributes = new Dictionary<string, object>
                    {
                        ["service.name"] = Configuration.ServiceName,
                    };
                });
        });
    }

    public static void AddCrossOrigin(this WebApplicationBuilder builder)
    {
        builder.Services.AddCors(options =>
        {
            options.AddPolicy(
                ApiConfiguration.CorsPolicyName,
                policy =>
                {
                    policy
                        .WithOrigins([Configuration.BackendUrl])
                        .AllowAnyMethod()
                        .AllowAnyHeader()
                        .AllowCredentials();
                });
        });
    }

    public static void AddServices(this WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<IValidator<CreateProductRequest>, CreateProductValidator>();
        builder.Services.AddScoped<ICreateProductHandler, CreateProductHandler>();
    }
}