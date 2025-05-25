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

// builder.Services.AddOpenTelemetry()
//     .ConfigureResource(resource => resource.AddService(builder.Environment.ApplicationName))
//     .WithTracing(tracing => tracing
//         .AddHttpClientInstrumentation()
//         .AddAspNetCoreInstrumentation()
//     )
//     .WithMetrics(metrics => metrics
//         .AddHttpClientInstrumentation()
//         .AddAspNetCoreInstrumentation()
//         .AddRuntimeInstrumentation())
//     .UseOtlpExporter();

// builder.Logging.AddOpenTelemetry(options =>
// {
//     options.IncludeScopes = true;
//     options.IncludeFormattedMessage = true;
// });

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

// app.UseCors(CorsOptions.PolicyName);

// app.UseAuthentication();
//
// app.UseAuthorization();

await app.RunAsync();
