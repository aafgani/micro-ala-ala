using System.Reflection;
using App.Api.Todo.Extensions;
using App.Common.Domain.Dtos;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.OpenApi.Models;
using App.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
var assembly = Assembly.GetExecutingAssembly();
var version = assembly.GetName().Version.ToString() ?? "1.0.0";

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Todo API",
        Version = version,
        Description = "A microservice for managing todo tasks",
        Contact = new OpenApiContact
        {
            Name = "Your Name",
            Email = "your.email@domain.com"
        }
    });

    // Add JWT Bearer Authentication
    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "Bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Enter your JWT token (without 'Bearer' prefix).\r\n\r\nExample: \"Bearer eyJhbGciOiJSUzI1NiIsInR5cCI6IkpXVCJ9...\""
    });

    options.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            new string[] {}
        }
    });
});

var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    await builder.AddKeyVaultSecretsAsync();
}

// Setup all the services.
builder.Services
    .AddApplicationInformation()
    .AddCustomAuthentication(builder.Configuration)
    .AddCustomAuthorization(builder.Configuration)
    .AddConfigurationOptions(builder.Configuration)
    .AddBusinesServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .ConfigureHealthChecks(builder.Configuration)
    .AddObservability("App.Api.Todo", builder.Configuration);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // ACA sets proper headers
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsProduction())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Todo API V1");
        c.RoutePrefix = "swagger";

        // Enable authorization persistence
        c.EnablePersistAuthorization();

        // Custom title with version info
        c.DocumentTitle = $"Todo API Documentation - Version {version}";
    });
}
else
{
    // Enforce HTTPS only in staging/production
    app.UseHttpsRedirection();
}

app.UseAuthentication();
app.UseAuthorization();

app.MapEndpoints();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
}).AllowAnonymous();

await app.RunAsync();

public partial class Program { }

