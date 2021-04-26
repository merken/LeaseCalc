using System;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using Newtonsoft.Json;
using LeaseCalc.Functions.Models;
using Mailjet.Client;
using Mailjet.Client.Resources;
using Mailjet.Client.TransactionalEmails;

namespace LeaseCalc.Functions
{
    public class SendEmailFunction
    {
        private static EmailMessage Deserialize(string message)
        {
            return JsonConvert.DeserializeObject<EmailMessage>(message);
        }

        [FunctionName("SendEmailFunction")]
        public static async Task Run(
            [QueueTrigger("leasecalc-email-queue", Connection = "AzureStorageConnection")] string message,
            ILogger log)
        {
            var emailMessage = Deserialize(message);
            var emailAddress = GetEnvironmentVariable("LeaseCalcMailerEmailAddress");
            var publicKey = GetEnvironmentVariable("MailJetPublicKey");
            var privateKey = GetEnvironmentVariable("MailJetPrivateKey");

            log.LogInformation($"Triggered SendEmailFunction with {message}");

            MailjetClient client = new MailjetClient(publicKey, privateKey);

            MailjetRequest request = new MailjetRequest
            {
                Resource = Send.Resource
            };

            var subject = $"Lease Quote for your {emailMessage.Context.VehicleInformation.Brand} {emailMessage.Context.VehicleInformation.Model} {emailMessage.Context.LeaseCalculationInformation.MonthlyLeasePrice}";
            var body = $"<pre id=\"json\">{JsonConvert.SerializeObject(emailMessage.Context)}</pre><script type=\"text/javascript\">document.getElementById(\"json\").innerHTML = JSON.stringify(data, undefined, 2);</script>";
            var to = emailMessage.Context.CustomerInformation.EmailAddress;
            var email = new TransactionalEmailBuilder()
                .WithFrom(new SendContact(emailAddress))
                .WithSubject(subject)
                .WithHtmlPart(body)
                .WithTo(new SendContact(to))
                .Build();

            log.LogInformation($"Sending email to {to} with subject {subject} and body {body}");

            await client.SendTransactionalEmailAsync(email);

            log.LogInformation($"Finished SendEmailFunction");
        }

        public static string GetEnvironmentVariable(string name)
        {
            return System.Environment.GetEnvironmentVariable(name, EnvironmentVariableTarget.Process);
        }
    }
}
