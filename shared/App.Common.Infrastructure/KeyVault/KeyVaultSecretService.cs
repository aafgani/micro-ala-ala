using App.Common.Domain;
using Azure;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;

namespace App.Common.Infrastructure.KeyVault
{
    public class KeyVaultSecretService : IKeyVaultSecretService
    {
        private readonly ILogger<KeyVaultSecretService> _logger;
        private readonly AsyncRetryPolicy<Response<KeyVaultSecret>> retryPolicy;
        private readonly IKeyVaultClient _keyVaultClient;


        public KeyVaultSecretService(IKeyVaultClient keyVaultClient, IOptions<CustomRetryPolicy> retryPolicyOptions, ILogger<KeyVaultSecretService> logger)
        {
            var configRetry = retryPolicyOptions.Value;
            _keyVaultClient = keyVaultClient;
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
                var response = await retryPolicy.ExecuteAsync(() => _keyVaultClient.GetSecretAsync(name));
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
