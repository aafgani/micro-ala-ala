using App.Common.Domain;
using Azure;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace App.Common.Infrastructure.KeyVault
{
    public class KeyVaultSecretService : IKeyVaultSecretService
    {
        private readonly SecretClient _client;
        private readonly ILogger<KeyVaultSecretService> _logger;
        private readonly AsyncRetryPolicy<Response<KeyVaultSecret>> retryPolicy;

        public KeyVaultSecretService(IOptions<EntraConfiguration> entraOptions, IOptions<CustomRetryPolicy> retryPolicyOptions, string KeyVaultUrl, ILogger<KeyVaultSecretService> logger)
        {
            var entra = entraOptions.Value;
            var configRetry = retryPolicyOptions.Value;

            var tenantId = entra.TenantId;
            var clientId = entra.ClientId;
            var clientSecret = entra.ClientSecret;

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _client = new SecretClient(new Uri(KeyVaultUrl), credential);
            _logger = logger;

            // Define a Polly retry policy with logging
            retryPolicy = Policy<Response<KeyVaultSecret>>
               .Handle<RequestFailedException>(ex => IsTransientError(ex))
               .WaitAndRetryAsync(
               retryCount: configRetry.MaxRetryCount,
               sleepDurationProvider: attempt => TimeSpan.FromSeconds(Math.Pow(configRetry.DelaySeconds, attempt)),
               onRetry: (exception, delay, retryCount, context) =>
               {
                   _logger.LogError($"Retry {retryCount} after {delay.TotalSeconds}s due to: {exception.Exception.Message}");
               });

        }

        public async Task<string> GetSecretAsync(string name)
        {
            try
            {
                var response = await retryPolicy.ExecuteAsync(() => _client.GetSecretAsync(name));
                return response.Value.Value;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Failed to retrieve secret: {SecretName}", name);
                throw;
            }
        }

        private bool IsTransientError(RequestFailedException ex)
        {
            return ex.Status == 429 || (ex.Status >= 500 && ex.Status < 600);
        }
    }
}
