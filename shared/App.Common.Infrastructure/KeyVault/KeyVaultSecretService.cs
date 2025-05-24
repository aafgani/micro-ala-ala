using App.Common.Domain;
using Azure;
using Azure.Core;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace App.Common.Infrastructure.KeyVault
{
    public class KeyVaultSecretService : IKeyVaultSecretService
    {
        private readonly SecretClient _client;
        private readonly ILogger<KeyVaultSecretService> _logger;

        public KeyVaultSecretService(IOptions<EntraConfiguration> entraOptions, string KeyVaultUrl, ILogger<KeyVaultSecretService> logger)
        {
            var entra = entraOptions.Value;
            var options = new SecretClientOptions
            {
                Retry =
                {
                    Mode = RetryMode.Exponential,
                    Delay = TimeSpan.FromSeconds(1),
                    MaxDelay = TimeSpan.FromSeconds(16),
                    MaxRetries = 5,
                    NetworkTimeout = TimeSpan.FromSeconds(100)
                }
            };
            options.Diagnostics.IsLoggingContentEnabled = true;

            var tenantId = entra.TenantId;
            var clientId = entra.ClientId;
            var clientSecret = entra.ClientSecret;

            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _client = new SecretClient(new Uri(KeyVaultUrl), credential, options);
            _logger = logger;
        }

        public async Task<string> GetSecretAsync(string name)
        {
            try
            {
                var response = await _client.GetSecretAsync(name);
                return response.Value.Value;
            }
            catch (RequestFailedException ex)
            {
                _logger.LogError(ex, "Failed to retrieve secret: {SecretName}", name);
                throw;
            }
        }
    }
}
