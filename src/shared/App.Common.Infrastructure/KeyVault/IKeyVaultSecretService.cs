namespace App.Common.Infrastructure.KeyVault
{
    public interface IKeyVaultSecretService
    {
        Task<string> GetSecretAsync(string name);
    }
}
