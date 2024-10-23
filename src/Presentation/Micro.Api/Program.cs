using Micro.Api.Common;
using Micro.Api.Common.Endpoints;
using Micro.Api.Common.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurations();
builder.AddOpenTelemetry();
builder.AddLogger();
builder.AddAuth();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
    app.ConfigureDevEnviroment();

app.UseHttpsRedirection();

app.UseAuth();

app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapEndpoints();

await app.InitializeDatabaseAsync();

await app.RunAsync();