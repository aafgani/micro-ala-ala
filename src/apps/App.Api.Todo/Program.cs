using System.Diagnostics;
using System.Reflection;
using App.Api.Todo.Extensions;
using HealthChecks.UI.Client;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.HttpOverrides;

var asm = Assembly.GetExecutingAssembly();
var location = asm.Location;
var version = asm.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion ?? "unknown";
var fileVersion = FileVersionInfo.GetVersionInfo(asm.Location).FileVersion;
var asmVersion = asm.GetName().Version;

Console.WriteLine($"🧩 Loaded from: {location}");
Console.WriteLine($"📦 AssemblyInformationalVersion: {version}");
Console.WriteLine($"📦 FileVersion: {fileVersion}");
Console.WriteLine($"📦 AssemblyVersion: {asmVersion}");


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var keyVaultUrl = builder.Configuration["KeyVaultUrl"];
if (!string.IsNullOrEmpty(keyVaultUrl))
{
    await builder.AddKeyVaultSecretsAsync();
}

// Setup all the services.
builder.Services
    .AddApplicationInformation()
    .AddConfigurationOptions(builder.Configuration)
    .AddBusinesServices(builder.Configuration)
    .AddInfrastructureServices(builder.Configuration)
    .ConfigureHealthChecks(builder.Configuration);

builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // ACA sets proper headers
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapEndpoints();
app.MapHealthChecks("/health", new HealthCheckOptions
{
    ResponseWriter = UIResponseWriter.WriteHealthCheckUIResponse
});
await app.RunAsync();

public partial class Program { }

