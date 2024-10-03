using Micro.Core.Common;
using Serilog;

namespace Micro.Api.Common.Extensions;

public static class BuildExtensions
{
    public static void AddConfigurations(this WebApplicationBuilder builder)
    {
        ApiConfiguration.ConnectionString =
            builder.Configuration.GetConnectionString("DefaultConnection") ?? string.Empty;
        Configuration.BackendUrl = builder.Configuration.GetValue<string>("Services:BackendUrl") ?? string.Empty;
    }

    public static void AddDocumentation(this WebApplicationBuilder builder)
    {
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen(x => { x.CustomSchemaIds(n => n.FullName); });
    }

    public static void AddLogger(this WebApplicationBuilder builder)
    {
        builder.Host.UseSerilog((_, configuration) =>
        {
            configuration
                .MinimumLevel.Information()
                .WriteTo.Console();
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
        
    }
}