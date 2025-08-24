using DockerHomeWorkApp.Core;
using DockerHomeWorkApp.DataContext;
using DockerHomeWorkApp.Endpoints;
using DockerHomeWorkApp.Middleware;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;
using System.Text.Json;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
    .AddEnvironmentVariables();

builder.Services.BindValidated<DbSettings>(builder.Configuration, "DbSettings");
builder.Services.AddDbContextPool<AppDataContext>((sp, options) =>
{
    var db = sp.GetRequiredService<IOptions<DbSettings>>().Value;
    options.UseNpgsql(db.ConnectionString).UseSnakeCaseNamingConvention();
});

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddProjectServices();
builder.Services.AddHealthChecks().AddNpgSql(sp => sp.GetRequiredService<IOptions<DbSettings>>().Value.ConnectionString, name: "npgsql");
builder.Services.AddSwaggerWithJwt();

var app = builder.Build();

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
});

app.UseErrorHandling();
app.UseMiddleware<RequestLoggingMiddleware>();

app.UseRouting();
app.UseHttpMetrics();
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

app.UseAuthentication();
app.UseAuthorization();

app.MapAuthEndpoints();
app.MapProfileEndpoints();
app.MapUserEndpoints();

app.Run();