using System.Diagnostics;
using Micro.Api.Common;
using Micro.Api.Common.Endpoints;
using Micro.Api.Common.Extensions;
using Micro.Core;

var builder = WebApplication.CreateBuilder(args);

builder.AddConfigurations();
builder.AddLogger();
builder.AddCrossOrigin();
builder.AddDocumentation();
builder.AddServices();

var app = builder.Build();

if (app.Environment.IsDevelopment()) 
    app.ConfigureDevEnviroment();

app.UseCors(ApiConfiguration.CorsPolicyName);
app.MapEndpoints();

await app.RunAsync();