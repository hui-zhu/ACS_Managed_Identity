using System;
using Azure.Identity;
using Azure.Communication.Identity;
using Azure.Communication.Sms;
using Azure.Core;
using Azure;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Net;
using Azure.Communication;

namespace ManagedIdentity
{
    class Program
    {

        static void Main(string[] args)
        {

            //DefaultAzureCredential defaultcredential = new DefaultAzureCredential();

            // You can find your endpoint and access key from your resource in the Azure portal
            // e.g. "https://<RESOURCE_NAME>.communication.azure.com";
            Uri endpoint = new Uri("https://youracsservice.communication.azure.com/");

            Console.WriteLine("Retrieving new Access Token, using Managed Identities");
            string clientId = "77c4d95a-767f-4088-bda1-Your client app id here";
            string tenantId = "your tenat id here";
            string secret = "your secret id here";

            ClientSecretCredential cred = new ClientSecretCredential(tenantId, clientId, secret);

            var client = new CommunicationIdentityClient(endpoint, cred);
            var identityResponse = client.CreateUser();
            var identity = identityResponse.Value;

            Response<AccessToken> tokenResponse = client.GetToken(identity, scopes: new[] { CommunicationTokenScope.VoIP });

            Console.WriteLine($"Retrieved Access Token: {tokenResponse.Value.Token}");

            Console.WriteLine("Sending SMS using Managed Identities");

            // You will need a phone number from your resource to send an SMS.
            SmsSendResult result = SendSms(endpoint, "+18445792722", "target phone", "Hello from Managed Identities", cred);
            Console.WriteLine($"Sms id: {result.MessageId}");
            Console.WriteLine($"Send Result Successful: {result.Successful}");
        }

        static public SmsSendResult SendSms(Uri resourceEndpoint, string from, string to, string message, TokenCredential myCredential)
        {

            SmsClient smsClient = new SmsClient(resourceEndpoint, myCredential);
            SmsSendResult sendResult = smsClient.Send(
                 from: from,
                 to: to,
                 message: message,
                 new SmsSendOptions(enableDeliveryReport: true) // optional
            );

            return sendResult;
        }
    }
}

