using App.Api.Todo.Features.Tags.Data;
using App.Api.Todo.Features.Tags.Mapper;
using App.Api.Todo.Features.Tags.Services;
using App.Api.Todo.Models;
using App.Common.Infrastructure.HealthCheck;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace App.Api.Todo.Extensions
{
    public static class ServiceCollectionExtensions
    {
        public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, ConfigurationManager configuration)
        {
            services
                .AddDbRepo(configuration)
                .AddMapper();

            return services;
        }

        public static IServiceCollection AddConfigurationOptions(this IServiceCollection services, ConfigurationManager configuration)
        {
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

        private static IServiceCollection AddDbRepo(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddDbContext<TodoContext>(options =>
            {
                options.UseSqlServer(configuration.GetConnectionString("TodoDb"));
            });
            services.AddScoped<ITagRepository, TagRepository>();

            return services;
        }

        public static IServiceCollection ConfigureHealthChecks(this IServiceCollection services, ConfigurationManager configuration)
        {
            services.AddHealthChecks()
                .AddCheck<DatabaseHealthCheck<TodoContext>>("Todo-database-health-check", failureStatus: HealthStatus.Unhealthy, tags: new[] { "database" });

            return services;
        }
    }
}
