using App.Web.Client.Utilities.Middleware;
using Microsoft.Identity.Web.UI;
using App.Web.Client.Extensions;
using Microsoft.AspNetCore.HttpOverrides;

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
    .AddInternalServices(config);

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
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStatusCodePagesWithReExecute("/error/?statusCode={0}");
app.UseStaticFiles();

app.UseRouting();
app.UseMiddleware<SessionValidationMiddleware>();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
