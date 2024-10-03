namespace Micro.Api.Common.Extensions;

public static class ApplicationExtensions
{
    public static void ConfigureDevEnviroment(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI();
    }
}