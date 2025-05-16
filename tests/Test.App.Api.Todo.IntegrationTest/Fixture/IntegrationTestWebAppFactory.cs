using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Models;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
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
            builder.ConfigureTestServices(services =>
            {
                var dbContextDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<TodoContext>));

                if (dbContextDescriptor != null)
                {
                    services.Remove(dbContextDescriptor);
                }

                services.AddDbContext<TodoContext>(options =>
                {
                    options.UseSqlServer(_dbFixture.ConnectionString);
                });

               
            });
        }

        public async Task DisposeAsync()
        {
            await _dbFixture.DisposeAsync();
        }
    }
}
