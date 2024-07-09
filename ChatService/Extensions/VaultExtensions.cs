using VaultSharp;
using VaultSharp.V1.AuthMethods.Token;

namespace ChatService.Extensions;

public static class VaultExtensions
{
    public static void LoadVaultSecrets(this IConfigurationBuilder builder, IConfiguration configuration, ILogger logger)
    {
        var vaultUri = configuration["Vault_URI"];
        var vaultToken = configuration["Vault_TOKEN"];

        var vaultClientSettings = new VaultClientSettings(vaultUri, new TokenAuthMethodInfo(vaultToken));
        var vaultClient = new VaultClient(vaultClientSettings);

        var secrets = vaultClient.V1.Secrets.KeyValue.V2.ReadSecretAsync(
            path: "secrets",
            mountPoint: "labchat"
        ).Result.Data.Data;

        foreach (var secret in secrets)
        {
            configuration[secret.Key] = secret.Value.ToString();
            logger.LogInformation($"Loaded secret {secret.Key} from Vault.");
        }
    }
}