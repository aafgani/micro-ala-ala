using Azure.Security.KeyVault.Secrets;
using Azure;
using App.Common.Domain;
using Microsoft.Extensions.Options;
using Azure.Identity;

namespace App.Common.Infrastructure.KeyVault
{
    public class KeyVaultClient : IKeyVaultClient
    {
        private readonly SecretClient _client;

        public KeyVaultClient(IOptions<EntraConfiguration> entraOptions, Uri keyVaultUrl)
        {
            var entra = entraOptions.Value;
            var tenantId = entra.TenantId;
            var clientId = entra.ClientId;
            var clientSecret = entra.ClientSecret;
            var credential = new ClientSecretCredential(tenantId, clientId, clientSecret);
            _client = new SecretClient(keyVaultUrl, credential);
        }

        public Task<Response<KeyVaultSecret>> GetSecretAsync(string name, CancellationToken cancellationToken = default)
        {
            return _client.GetSecretAsync(name, cancellationToken: cancellationToken);
        }
    }
}
