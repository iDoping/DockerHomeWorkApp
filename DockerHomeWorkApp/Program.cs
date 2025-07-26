using DockerHomeWorkApp.Core;
using DockerHomeWorkApp.DataContext;
using DockerHomeWorkApp.Endpoints;
using DockerHomeWorkApp.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Prometheus;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.Configure<DbSettings>(builder.Configuration.GetSection("DbSettings"));
builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<DbSettings>>().Value);

builder.Services.AddDbContext<AppDataContext>((sp, options) =>
{
    var dbSettings = sp.GetRequiredService<DbSettings>();
    options.UseNpgsql(dbSettings.ConnectionString)
           .UseSnakeCaseNamingConvention();
});

builder.Services.AddHealthChecks()
    .AddNpgSql(sp => sp.GetRequiredService<DbSettings>().ConnectionString, name: "npgsql");

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "User API", Version = "v1" });
});

builder.Services.AddProjectServices();

var app = builder.Build();

app.UseErrorHandling();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();

app.UseHttpMetrics();
app.UseMetricServer();

app.MapUserEndpoints();
app.MapMetrics();

app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = async (context, report) =>
    {
        context.Response.ContentType = "application/json";

        var response = new
        {
            status = report.Status.ToString(),
            checks = report.Entries.Select(e => new
            {
                name = e.Key,
                status = e.Value.Status.ToString(),
                error = e.Value.Exception?.Message
            })
        };

        await context.Response.WriteAsync(JsonSerializer.Serialize(response));
    }
});

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "User API v1");
        options.RoutePrefix = "swagger";
    });
}

app.Run();