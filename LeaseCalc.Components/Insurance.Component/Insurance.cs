using System;
using System.Linq;
using System.Collections.Generic;
using LeaseCalc.Contract;
using Prise.Plugin;
using System.Threading.Tasks;
using System.Net.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System.Text;

namespace Insurance.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component calculates the monthly insurance fee
    public class Insurance : ILeaseCalcComponent
    {
        [PluginService(ServiceType = typeof(IConfigurationService), ProvidedBy = ProvidedBy.Host, ProxyType = typeof(ConfigurationServiceProxy))]
        private readonly IConfigurationService configurationService;
        public async Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var endpoint = this.configurationService.GetConfigurationValueForKey("InsuranceApi");
            var httpClient = new HttpClient();
            httpClient.BaseAddress = new Uri(endpoint);

            var payload = JsonConvert.SerializeObject(context, new JsonSerializerSettings
            {
                ContractResolver = new DefaultContractResolver()
                {
                    NamingStrategy = new CamelCaseNamingStrategy()
                }
            });

            var response = await httpClient.PostAsync("/insurance", new StringContent(payload, Encoding.UTF8, "application/json"));
            if (!response.IsSuccessStatusCode)
                throw new LeaseCalcException($"Response from Insurance service was not OK");

            var insuranceCost = decimal.Parse((await response.Content.ReadAsStringAsync()));

            // Add the result from the insurance backend
            context.LeaseCalculationInformation.MonthlyLeasePrice += insuranceCost;

            return context;
        }
    }
}
