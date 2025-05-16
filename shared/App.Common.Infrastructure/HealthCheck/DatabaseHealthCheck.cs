using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Diagnostics.HealthChecks;

namespace App.Common.Infrastructure.HealthCheck
{
    public class DatabaseHealthCheck<TContext> : IHealthCheck
        where TContext : DbContext
    {
        private readonly IServiceProvider _serviceProvider;

        public DatabaseHealthCheck(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<HealthCheckResult> CheckHealthAsync(HealthCheckContext context, CancellationToken cancellationToken = default)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<TContext>();

                try
                {
                    var isHealthy = await dbContext.Database.CanConnectAsync(cancellationToken);
                    return isHealthy ?
                        HealthCheckResult.Healthy("Database connection is healthy") :
                        HealthCheckResult.Unhealthy("Database connection failed");
                }
                catch (Exception ex)
                {
                    return HealthCheckResult.Unhealthy("Database connection failed", ex);
                }

            }
        }
    }
}
