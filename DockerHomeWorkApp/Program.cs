using DockerHomeWorkApp.Core;
using DockerHomeWorkApp.DataContext;
using DockerHomeWorkApp.Endpoints;
using DockerHomeWorkApp.Middleware;
using DockerHomeWorkApp.Services;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Prometheus;

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

var billingBaseUrl = builder.Configuration["Billing:BaseUrl"] ?? "http://billing-service";

builder.Services.AddHttpClient<IBillingClient, BillingClient>(client =>
{
    client.BaseAddress = new Uri(billingBaseUrl);
});

builder.Services.AddJwtAuth(builder.Configuration);
builder.Services.AddProjectServices();
builder.Services.AddHealthChecks()
    .AddNpgSql(sp => sp.GetRequiredService<IOptions<DbSettings>>().Value.ConnectionString, name: "DbSettings");
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

app.MapHealthChecks("/health");

app.UseAuthentication();
app.UseAuthorization();

app.UseSwagger();
app.UseSwaggerUI();

app.MapAuthEndpoints();
app.MapProfileEndpoints();
app.MapUserEndpoints();

app.Run();