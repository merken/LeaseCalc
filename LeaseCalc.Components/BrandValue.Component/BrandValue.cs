using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaseCalc.Contract;
using Prise.Plugin;

namespace BrandValue.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// Modifies the residual value based on the degradation of the brand value
    public class BrandValue : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var residualValue = context.LeaseCalculationInformation.VehicleResidualValue;
            var brand = context.VehicleInformation.Brand;
            switch (brand)
            {
                default: // All others
                    residualValue *= 0.15m;
                    break;
                case "RENAULT":
                case "CITROEN":
                case "PEUGEOT":
                    residualValue *= 0.05m;
                    break;
                case "VOLKSWAGEN":
                case "SKODA":
                    residualValue *= 0.04m;
                    break;
                case "AUDI":
                case "MERCEDES":
                case "BWM":
                    residualValue *= 0.03m;
                    break;
                case "JAGUAR":
                case "LANDROVER":
                case "TESLA":
                    residualValue *= 0.02m;
                    break;
            }

            context.LeaseCalculationInformation.VehicleResidualValue = residualValue;
            
            return Task.FromResult(context);
        }
    }
}
