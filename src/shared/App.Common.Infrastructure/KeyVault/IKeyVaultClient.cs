using Azure.Security.KeyVault.Secrets;
using Azure;

namespace App.Common.Infrastructure.KeyVault
{
    public interface IKeyVaultClient
    {
        Task<Response<KeyVaultSecret>> GetSecretAsync(string name, CancellationToken cancellationToken = default);
    }

}
