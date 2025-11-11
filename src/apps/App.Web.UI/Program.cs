using App.Web.UI.Extensions;
using App.Web.UI.Utilities.Session;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Identity.Web.UI;
using App.Diagnostics;

var builder = WebApplication.CreateBuilder(args);
var config = builder.Configuration
    .AddEnvironmentVariables()
    .Build();

// Add services to the container.
builder.Services
    .AddControllersWithViews()
    .AddMicrosoftIdentityUI();

builder.Services
.AddCustomAuthentication(config)
.AddCustomAuthorization(config)
.AddInternalServices(config)
.AddObservability("App.Web.UI", builder.Configuration);

// Handle reverse proxy headers
builder.Services.Configure<ForwardedHeadersOptions>(options =>
{
    options.ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto;
    options.KnownNetworks.Clear(); // ACA sets proper headers
    options.KnownProxies.Clear();
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
var pathBase = config.GetValue<string>("PathBase");
if (!string.IsNullOrEmpty(pathBase))
{
    app.UsePathBase(pathBase);
}
app.UseRouting();

app.UseStatusCodePagesWithReExecute("/error", "?statusCode={0}");

app.UseForwardedHeaders();
app.UseAuthentication();
app.UseAuthorization();
app.UseMiddleware<SessionValidationMiddleware>();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");


app.Run();
