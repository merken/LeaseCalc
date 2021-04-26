using Microsoft.Azure.WebJobs;
using Microsoft.Extensions.Logging;
using LeaseCalc.Contract;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.Linq;
using System;
using System.IO;
using Prise;
using LeaseCalc.Functions.Models;

namespace LeaseCalc.Functions
{
    public class ComponentFunction
    {
        private readonly IPluginLoader pluginLoader;
        private readonly INugetServerService nugetServerService;
        private readonly IConfigurationService configurationService;

        public ComponentFunction(
            IPluginLoader pluginLoader, 
            INugetServerService nugetServerService, 
            IConfigurationService configurationService)
        {
            this.pluginLoader = pluginLoader;
            this.nugetServerService = nugetServerService;
            this.configurationService = configurationService;
        }

        private static ComponentMessage Deserialize(string message)
        {
            return JsonConvert.DeserializeObject<ComponentMessage>(message);
        }

        [FunctionName("ComponentFunction")]
        public async Task Run(
            [QueueTrigger("leasecalc-component-queue", Connection = "AzureStorageConnection")] string message,
            [Queue("leasecalc-journey-queue"), StorageAccount("AzureStorageConnection")] ICollector<string> journeyQueue,
            ILogger log)
        {
            var componentMessage = Deserialize(message);
            var componentName = componentMessage.ComponentName;
            var componentVersion = componentMessage.ComponentVersion;

            log.LogInformation($"Component {componentName} received context {componentMessage.Context}");

            var nugetComponentVersions = await this.nugetServerService.GetPackageVersions(componentName);
            if (!nugetComponentVersions.Any())
                throw new ArgumentException($"Component {componentName} does not have any versions on NuGet");

            var nugetComponent = nugetComponentVersions.FirstOrDefault(c => c.Version.Equals(new Version(componentVersion)));
            if (nugetComponent == null)
                throw new ArgumentException($"Component {componentName} with version {componentVersion} does not exist on NuGet");

            var packageName = $"{componentName}.{componentVersion}.nupkg";
            var packageLocation = Path.GetFullPath(packageName); // Results to bin/Debug/netcoreapp3.1/package.version.nupkg

            if(IsRunningOnAzure())
                packageLocation = Path.Combine(Path.GetTempPath(), packageName); // Results to the TEMP directory of the Azure Functions (App Service) on Azure

            if(!File.Exists(packageLocation)) // Download from NuGet server if file with specific version does not exist
            {
                var package = await this.nugetServerService.DownloadPackage(componentName, componentVersion);
                await File.WriteAllBytesAsync(packageLocation, package);
            }

            var pathToPackage = Path.GetDirectoryName(packageLocation); // Returns the root execution directory, where all nupkg's are downloaded
            // Scanning for nupkg's also extracts all available packages
            var pluginScanResults = await this.pluginLoader.FindPlugins<ILeaseCalcComponent>(pathToPackage);
            // We're looking for the nupkg with the correct name and version
            var pluginScanResult = pluginScanResults.FirstOrDefault(p => p.AssemblyPath.Contains(componentName) && p.AssemblyPath.Contains(componentVersion));
            if(pluginScanResult == null)
                throw new NotSupportedException($"Could not find extracted plugin with name {componentName} and version {componentVersion}");

            var plugin = await this.pluginLoader.LoadPlugin<ILeaseCalcComponent>(pluginScanResult, configure: (ctx) =>
            {
                // Share the IConfigurationService
                ctx.AddHostService<IConfigurationService>(this.configurationService);
            });

            var componentBreadCrumb = $"{componentName},{componentVersion}";
            var newContext = AddBreadCrumb(await plugin.Calculate(componentMessage.Context), componentBreadCrumb);

            log.LogInformation($"Component {componentBreadCrumb} created new context {newContext}");

            journeyQueue.Add(JsonConvert.SerializeObject(newContext));
        }

        private bool IsRunningOnAzure()
        {
            return this.configurationService.GetConfigurationValueForKey("AZURE") != null && 
                bool.Parse(this.configurationService.GetConfigurationValueForKey("AZURE"));
        }

        private LeaseCalcContext AddBreadCrumb(LeaseCalcContext context, string component)
        {
            var breadCrumbs = new List<string>(context.JourneyBreadcrumbs ?? new string[] { });
            breadCrumbs.Add(component);
            context.JourneyBreadcrumbs = breadCrumbs.ToArray();
            return context;
        }
    }
}
