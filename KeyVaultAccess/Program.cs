using Microsoft.Azure.KeyVault;
using Microsoft.Azure.Services.AppAuthentication;
using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace KeyVaultAccess
{
    class Program
    {
        static void Main(string[] args)
        {
            var azureServiceTokenprovider = new AzureServiceTokenProvider();
            KeyVaultClient client = new KeyVaultClient(new KeyVaultClient.AuthenticationCallback(azureServiceTokenprovider.KeyVaultTokenCallback));
            var secret = client.GetSecretAsync("https://azurelearningvault89.vault.azure.net/secrets/tablestorageconnectionstring/e7c5dfe6a7a04bb980030f86fa658484")
                .ConfigureAwait(false)
                .GetAwaiter()
                .GetResult();

        }

        private async static Task<string> GetAccessTokenForKV(string a, string b, string c)
        {
            const string ClientId = "09d5540f-6d91-4d5e-8f96-2d00d915f221";
            const string ClientSecret = "3f-M9P7VtLT6x8E98~G_ZqXTbQ3JGcz_Zn";
            const string TenantId = "549a973a-fda2-4190-9ee8-67445857b006";

            var app = ConfidentialClientApplicationBuilder.Create(ClientId)
                .WithAuthority(AzureCloudInstance.AzurePublic, TenantId)
                .WithClientSecret(ClientSecret)
                .Build();

            var scopes = new List<string>() { "https://vault.azure.net/.default" };

            AuthenticationResult result =  await app.AcquireTokenForClient(scopes).ExecuteAsync().ConfigureAwait(false);
            return result.AccessToken;
        }
    }
}
