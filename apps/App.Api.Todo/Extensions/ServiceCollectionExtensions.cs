using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Mapper;
using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using App.Common.Domain;
using App.Common.Infrastructure.HealthCheck;
using App.Common.Infrastructure.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace App.Api.Todo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static async Task<IConfigurationBuilder> AddKeyVaultSecretsAsync(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var services = builder.Services;
            services
                .AddOptions<EntraConfiguration>()
                .Bind(configuration.GetSection("Entra"))
                .ValidateDataAnnotations();
            using var tempProvider = services.BuildServiceProvider();

            var secretService = new KeyVaultSecretService(
                tempProvider.GetRequiredService<IOptions<EntraConfiguration>>(),
                 configuration["KeyVaultUrl"],
                 tempProvider.GetRequiredService<ILogger<KeyVaultSecretService>>());

            var kvSecrets = new Dictionary<string, string>
            {
                ["ConnectionStrings:TodoDb"] = await secretService.GetSecretAsync("todo-connection-string")
            };

            return builder.Configuration.AddInMemoryCollection(kvSecrets);
        }

        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddDbRepo(configuration)
                .AddMapper();

            return services;
        }

        public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, IConfiguration configuration)
        {


            return services;
        }

        public static Dictionary<string,string> GetKeyVaultSecrets(IServiceCollection services)
        {
            var memoryConfig = new Dictionary<string, string>();

            return memoryConfig;
        }

        public static IServiceCollection AddBusinesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITagService, TagService>();

            return services;
        }

        private static IServiceCollection AddMapper(this IServiceCollection services)
        {
            services.AddSingleton<ITagMapper, TagMapper>();

            return services;
        }

        private static IServiceCollection AddDbRepo(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TodoContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("TodoDb"));
            });
            services.AddScoped<ITagRepository, TagRepository>();

            return services;
        }

        public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck<TodoContext>>("Todo-database-health-check", failureStatus: HealthStatus.Unhealthy, tags: new[] { "database" });

            return services;
        }
    }
}
