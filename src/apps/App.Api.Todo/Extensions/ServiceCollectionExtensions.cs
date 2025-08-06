using App.Api.Todo.Configuration;
using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Mapper;
using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Features.Todolist.Data;
using App.Api.Todo.Features.Todolist.Mapper;
using App.Api.Todo.Features.Todolist.Services;
using App.Api.Todo.Features.Todos.Data;
using App.Api.Todo.Features.Todos.Mapper;
using App.Api.Todo.Features.Todos.Services;
using App.Api.Todo.Features.Todotask.Data;
using App.Api.Todo.Features.Todotask.Mapper;
using App.Api.Todo.Features.Todotask.Services;
using App.Api.Todo.Models;
using App.Common.Domain;
using App.Common.Domain.Configuration;
using App.Common.Infrastructure.HealthCheck;
using App.Common.Infrastructure.KeyVault;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Options;

namespace App.Api.Todo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddApplicationInformation(this IServiceCollection services)
        {
            services.AddSingleton(implementationFactory =>
           {
               var hostEnvironment = implementationFactory.GetRequiredService<IHostEnvironment>();

               var applicationInformation = new ApplicationInformation(hostEnvironment);

               return applicationInformation;
           });

            return services;
        }

        public static async Task<IConfigurationBuilder> AddKeyVaultSecretsAsync(this WebApplicationBuilder builder)
        {
            var configuration = builder.Configuration;
            var services = builder.Services;
            services
                .AddOptions<EntraConfiguration>()
                .Bind(configuration.GetSection("Entra"))
                .ValidateDataAnnotations();
            services
                .AddOptions<CustomRetryPolicy>()
                .Bind(configuration.GetSection("RetryPolicy"))
                .ValidateDataAnnotations();

            using var tempProvider = services.BuildServiceProvider();

            var keyVaultUrl = configuration["KeyVaultUrl"];
            if (string.IsNullOrWhiteSpace(keyVaultUrl))
            {
                throw new InvalidOperationException("KeyVaultUrl configuration value is missing or empty.");
            }
            IKeyVaultClient keyVaultClient = new KeyVaultClient(
                tempProvider.GetRequiredService<IOptions<EntraConfiguration>>(),
                new Uri(keyVaultUrl));

            var secretService = new KeyVaultSecretService(
                keyVaultClient,
                tempProvider.GetRequiredService<IOptions<CustomRetryPolicy>>(),
                 tempProvider.GetRequiredService<ILogger<KeyVaultSecretService>>());

            var kvSecrets = new Dictionary<string, string>
            {
                ["ConnectionStrings:TodoDb"] = await secretService.GetSecretAsync("todo-connection-strings")
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

        public static Dictionary<string, string> GetKeyVaultSecrets(IServiceCollection services)
        {
            var memoryConfig = new Dictionary<string, string>();

            return memoryConfig;
        }

        public static IServiceCollection AddBusinesServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<ITagService, TagService>();
            services.AddScoped<ITodoTaskService, TodoTaskService>();
            services.AddScoped<ITodolistService, TodolistService>();
            services.AddScoped<ITodoService, TodoService>();
            return services;
        }

        private static IServiceCollection AddMapper(this IServiceCollection services)
        {
            services.AddSingleton<ITagMapper, TagMapper>();
            services.AddSingleton<ITaskMapper, TaskMapper>();
            services.AddSingleton<ITodoListMapper, TodoListMapper>();
            services.AddSingleton<ITodoMapper, TodoMapper>();

            return services;
        }

        private static IServiceCollection AddDbRepo(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<TodoContext>(options =>
            {
                options.UseNpgsql(configuration.GetConnectionString("TodoDb"));
            });
            services.AddScoped<ITagRepository, TagRepository>();
            services.AddScoped<ITodoTaskRepository, TodoTaskRepository>();
            services.AddScoped<ITodolistRepository, TodolistRepository>();
            services.AddScoped<ITodoRepository, TodoRepository>();

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
