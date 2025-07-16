using App.Api.Todo.Models;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Test.App.Todo.Integration.Helper;
using Task = System.Threading.Tasks.Task;

namespace Test.App.Api.Todo.IntegrationTest.Fixture
{
    public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>, IAsyncLifetime
    {
        protected DatabaseContainerFixture _dbFixture = new();

        public async Task InitializeAsync()
        {
            await _dbFixture.InitializeAsync();
        }

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            // Set environment variable
            // to enable authentication in tests
            // This allows us to test endpoints that require authentication
            Environment.SetEnvironmentVariable("EnableAuthentication", "true");

            builder.ConfigureTestServices(services =>
            {
                // Add test authentication
                services.AddAuthentication("Test")
                    .AddScheme<AuthenticationSchemeOptions, TestAuthenticationHandler>("Test", options => { });

                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TodoContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<TodoContext>(options =>
                {
                    options.UseNpgsql(_dbFixture.ConnectionString);
                });
            });

            builder.UseEnvironment("Testing");
        }

        public async Task DisposeAsync()
        {
            await _dbFixture.DisposeAsync();
        }
    }
}
