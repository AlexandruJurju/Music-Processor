using System.Reflection;
using MusicProcessor.Application;
using HealthChecks.UI.Client;
using MusicProcessor.Infrastructure;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using MusicProcessor.WebApi;
using MusicProcessor.WebApi.Extensions;
using Scalar.AspNetCore;
using Serilog;

WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((context, loggerConfig) => loggerConfig.ReadFrom.Configuration(context.Configuration));

builder.Services
    .AddPresentation(builder.Configuration)
    .AddApplication(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.AddEndpoints(Assembly.GetExecutingAssembly());

WebApplication app = builder.Build();

app.MapEndpoints();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();

    // app.UseSwaggerUI(options =>
    // {
    //     options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
    //     options.RoutePrefix = string.Empty;
    // });

    app.MapScalarApiReference(options => options.WithOpenApiRoutePattern("/swagger/v1/swagger.json"));

    app.ApplyMigrations();
}

app.MapHealthChecks("health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});

app.UseHttpsRedirection();

app.UseRequestContextLogging();

app.UseSerilogRequestLogging();

app.UseExceptionHandler();

await app.RunAsync();
