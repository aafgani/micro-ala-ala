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
        public static IServiceCollection AddKeyVaultServices(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddOptions<EntraConfiguration>()
                .Bind(configuration.GetSection("Entra"))
                .ValidateDataAnnotations();

            services.AddSingleton<IKeyVaultSecretService>(sp =>
            {
                return new KeyVaultSecretService(
                    sp.GetRequiredService<IOptions<EntraConfiguration>>(),
                    configuration["KeyVaultUrl"],
                    sp.GetRequiredService<ILogger<KeyVaultSecretService>>()
                );
            });
            return services;
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

        public static async Task<IServiceCollection> AddConfigurationOptionsAsync(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddKeyVaultServices(configuration);

            // Build a temp provider to resolve services early
            using var tempProvider = services.BuildServiceProvider();
            var secretService = tempProvider.GetRequiredService<IKeyVaultSecretService>();
            var connectionString = await secretService.GetSecretAsync("todo-connection-string");

            services.Configure<MySecrets>(opts =>
            {
                opts.TodoConnectionString = connectionString;
            });

            return services;
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
            var mySecrets = new MySecrets();
            configuration.Bind("MySecrets", mySecrets);

            services.AddDbContext<TodoContext>(options =>
            {
                options.UseSqlServer(mySecrets.TodoConnectionString);
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
