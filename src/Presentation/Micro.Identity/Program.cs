using Micro.Identity.Extensions;
using Microsoft.AspNetCore.Identity;

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
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapCustomIdentityApi<IdentityUser>();

await app.InitializeDatabaseAsync();

await app.RunAsync();