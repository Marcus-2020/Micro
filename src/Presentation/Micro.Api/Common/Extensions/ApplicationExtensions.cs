using System.Diagnostics;
using Npgsql;

namespace Micro.Api.Common.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigureDevEnviroment(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
    
    public static async Task InitializeDatabaseAsync(this WebApplication app)
    {
        var watch = Stopwatch.StartNew();
        var logger = Serilog.Log.Logger.ForContext<Program>();
        try
        {
            logger.Information("Initializing database at {DateTime}", DateTime.UtcNow);
            var connectionString = app.Configuration.GetConnectionString("DefaultConnection");
            await using var connection = new NpgsqlConnection(connectionString);
            await connection.OpenAsync();
        
            var commandText = await File.ReadAllTextAsync("Resources/init.sql");
            await using var commandInit = new NpgsqlCommand(commandText, connection);
            await commandInit.ExecuteNonQueryAsync();
            
            logger.Information("Database initialization complete in {ElapsedMilliseconds}ms",
                watch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.Error(ex, "An error occurred while initializing the database: {Message}", 
                ex.Message);
            throw;
        }
    }
}