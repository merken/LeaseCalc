using System;
using System.IO;
using System.Threading.Tasks;
using System.Linq;
using LeaseCalc.Contract;
using LeaseCalc.Functions.Models;
using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Blob;

namespace LeaseCalc.Functions
{
    public static class JourneyFunction
    {
        [FunctionName("JourneyFunction")]
        public static async Task Run(
            [QueueTrigger("leasecalc-journey-queue", Connection = "AzureStorageConnection")] string message,
            [Blob("journeys", FileAccess.Read, Connection = "AzureStorageConnection")] CloudBlobContainer journeysContainer,
            [Queue("leasecalc-component-queue"), StorageAccount("AzureStorageConnection")] ICollector<string> componentQueue,
            [Queue("leasecalc-email-queue"), StorageAccount("AzureStorageConnection")] ICollector<string> emailQueue,
            ILogger log)
        {
            log.LogInformation($@"Journey triggerred with message {message}");

            var context = DeserializeMessage(message);
            var journeyBlobFileName= $"{context.Journey}.json";

            if(!journeysContainer.GetBlockBlobReference(journeyBlobFileName).Exists())
                throw new NotSupportedException($"Journey {journeyBlobFileName} was not found.");

            var journeyFromBlob = await journeysContainer.GetBlockBlobReference(journeyBlobFileName).DownloadTextAsync();

            var journey = DeserializeJourney(journeyFromBlob);

            foreach (var component in journey.RentalPriceComponents ?? Enumerable.Empty<string>())
            {
                var componentName = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                var componentVersion = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (IsProcessed(component, context.JourneyBreadcrumbs))
                    continue;

                componentQueue.Add(JsonConvert.SerializeObject(new ComponentMessage
                {
                    Context = context,
                    ComponentName = componentName,
                    ComponentVersion = componentVersion
                }));
                return; // stop function execution
            }

            foreach (var component in journey.ResidualValueComponents ?? Enumerable.Empty<string>())
            {
                var componentName = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                var componentVersion = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (IsProcessed(component, context.JourneyBreadcrumbs))
                    continue;

                componentQueue.Add(JsonConvert.SerializeObject(new ComponentMessage
                {
                    Context = context,
                    ComponentName = componentName,
                    ComponentVersion = componentVersion
                }));
                return; // stop function execution
            }

            foreach (var component in journey.MonthlyLeaseComponents ?? Enumerable.Empty<string>())
            {
                var componentName = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                var componentVersion = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (IsProcessed(component, context.JourneyBreadcrumbs))
                    continue;

                componentQueue.Add(JsonConvert.SerializeObject(new ComponentMessage
                {
                    Context = context,
                    ComponentName = componentName,
                    ComponentVersion = componentVersion
                }));
                return; // stop function execution
            }

            foreach (var component in journey.ServicesComponents ?? Enumerable.Empty<string>())
            {
                var componentName = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[0];
                var componentVersion = component.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries)[1];
                if (IsProcessed(component, context.JourneyBreadcrumbs))
                    continue;

                componentQueue.Add(JsonConvert.SerializeObject(new ComponentMessage
                {
                    Context = context,
                    ComponentName = componentName,
                    ComponentVersion = componentVersion
                }));
                return; // stop function execution
            }

            // At the end, send an email with the results
            emailQueue.Add(JsonConvert.SerializeObject(new EmailMessage
            {
                Context = context
            }));
        }

        private static LeaseCalcContext DeserializeMessage(string message)
        {
            return JsonConvert.DeserializeObject<LeaseCalcContext>(message);
        }

        private static Journey DeserializeJourney(string journey)
        {
            return JsonConvert.DeserializeObject<JourneyRootObject>(journey).Journey;
        }

        private static bool IsProcessed(string component, string[] journeyBreadcrumbs)
        {
            if (journeyBreadcrumbs == null)
                return false;
            return Array.IndexOf(journeyBreadcrumbs, component) >= 0;
        }
    }
}