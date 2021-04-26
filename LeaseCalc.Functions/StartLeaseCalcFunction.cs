using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Text.Json;
using LeaseCalc.Contract;

namespace LeaseCalc.Functions
{
    public static class StartLeaseCalcFunction
    {
        [FunctionName("StartLeaseCalc")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Anonymous, "post", Route = "StartLeaseCalc/{journey}")] HttpRequest req,
            [Queue("leasecalc-journey-queue"), StorageAccount("AzureStorageConnection")] ICollector<string> queue,
            string journey,
            ILogger log)
        {
            var context = await JsonSerializer.DeserializeAsync<LeaseCalcContext>(new StreamReader(req.Body).BaseStream, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            context.Journey = journey;

            log.LogInformation($@"Starting the calculation for the initial rental price for vehicle 
            Journey: {journey} 
            Brand: {context.VehicleInformation.Brand} 
            Model: {context.VehicleInformation.Model}
            RetailPrice: {context.VehicleInformation.RetailPrice}");

            queue.Add(JsonSerializer.Serialize(context));

            return (ActionResult)new OkObjectResult($"Ok");
        }
    }
}
