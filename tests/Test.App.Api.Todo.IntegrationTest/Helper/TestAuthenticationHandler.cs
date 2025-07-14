using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Test.App.Todo.Integration.Helper;

public class TestAuthenticationHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    public TestAuthenticationHandler(IOptionsMonitor<AuthenticationSchemeOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
    {
    }

    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var authorizationHeader = Request.Headers["Authorization"].ToString();

        // ❌ No header = NoResult (let other auth handlers try, or fail if none)
        if (string.IsNullOrEmpty(authorizationHeader))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // ❌ Not a test token = NoResult (let JWT handler try)
        if (!authorizationHeader.StartsWith("Test "))
        {
            return Task.FromResult(AuthenticateResult.NoResult());
        }

        // Only handle "Test" scheme - let real JWT tokens pass through to JWT handler
        if (string.IsNullOrEmpty(authorizationHeader) || !authorizationHeader.StartsWith("Test "))
        {
            return Task.FromResult(AuthenticateResult.NoResult()); // Let other handlers try
        }

        // ✅ Test token = Success
        var userId = authorizationHeader.Substring("Test ".Length);

        var claims = new[]
        {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Name, $"TestUser{userId}"),
                new Claim(ClaimTypes.Email, $"test{userId}@example.com"),
                new Claim("oid", userId), // Azure AD Object ID
                new Claim("tid", "test-tenant-id"), // Tenant ID
                new Claim(ClaimTypes.Role, "Todos:View"),
                new Claim(ClaimTypes.Role, "Todos:Contributor"),
                new Claim(ClaimTypes.Role, "Todos:Admin")
            };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");

        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
