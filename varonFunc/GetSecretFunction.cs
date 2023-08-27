
using System;
using System.IO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.Http;
using Microsoft.Azure.WebJobs.Extensions.Http;

public static class GetSecretFunction
{
    [FunctionName("GetSecret")]
    public static IActionResult Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = null)] HttpRequest req,
        ILogger log)
    {
        log.LogInformation("C# HTTP trigger function processed a request.");

        string secretName = req.Query["name"];

        // Get the credentials using DefaultAzureCredential
        var credential = new ManagedIdentityCredential();

        // Specify the Key Vault name
        string keyVaultName = "VaronisAssignmentKv1";

        // Initialize SecretClient with the Key Vault URL and credentials
        var secretClient = new SecretClient(new Uri($"https://{keyVaultName}.vault.azure.net"), credential);

        try
        {
            // Get the secret value by secret name
            KeyVaultSecret secret = secretClient.GetSecret(secretName);

            // Prepare the response
            var value = new
            {
                KeyVaultName = keyVaultName,
                SecretName = secret.Name,
                CreationDate = secret.Properties.CreatedOn,
                SecretValue = secret.Value
            };
            var response = value;

            return new OkObjectResult(response);
        }
        catch (Exception e)
        {
            return new BadRequestObjectResult(e.Message);
        }
    }
}
