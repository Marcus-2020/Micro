using System.Diagnostics;
using Micro.Identity.Data;
using Microsoft.EntityFrameworkCore;

namespace Micro.Identity.Extensions;

public static class ApplicationExtensions
{
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var watch = Stopwatch.StartNew();
        var logger = Serilog.Log.Logger.ForContext<Program>();
            
        logger.Information("Initializing database at {DateTime}", DateTime.UtcNow);
            
        var services = scope.ServiceProvider;
        var context = services.GetRequiredService<ApplicationDbContext>();

        try
        {
            await context.Database.MigrateAsync();
            logger.Information("Database initialization complete in {ElapsedMilliseconds}ms",
                watch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while migrating or initializing the database: {Message}", 
                ex.Message);
            throw;
        }
    } 
}