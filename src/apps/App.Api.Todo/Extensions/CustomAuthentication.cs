using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

namespace App.Api.Todo.Extensions;

public static class CustomAuthentication
{
    public static IServiceCollection AddCustomAuthentication(this IServiceCollection services, IConfiguration configuration)
    {
        var azureEntra = configuration.GetSection("AzureEntra");
        var tenantId = azureEntra["TenantId"];

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
            options.Authority = $"{azureEntra["Instance"]}{azureEntra["TenantId"]}/v2.0";
            options.Audience = azureEntra["Audience"]; // ✅  explicitly set

            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true, // ✅ Ensure this is true
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ClockSkew = TimeSpan.FromMinutes(5),
                // Accept both v1.0 and v2.0 issuers
                ValidIssuers = new[]
                {
                    $"https://sts.windows.net/{tenantId}/",           // v1.0
                    $"https://login.microsoftonline.com/{tenantId}/v2.0"  // v2.0
                },
            };
        });

        services.AddAuthorization();

        return services;
    }

}
