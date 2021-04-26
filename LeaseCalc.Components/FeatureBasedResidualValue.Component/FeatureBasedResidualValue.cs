using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaseCalc.Contract;
using Prise.Plugin;

namespace FeatureBasedResidualValue.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component modifies the residual value based on what features a vehicle has
    public class FeatureBasedResidualValue : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var residualValue = context.LeaseCalculationInformation.VehicleResidualValue;
            foreach (var feature in context.VehicleInformation.Features)
            {
                switch (feature.FeatureCode)
                {
                    case "NO_AIRCO":
                        residualValue *= 0.05m;
                        break;
                    case "4_X_4":
                        residualValue *= 0.10m;
                        break;
                    case "TOW_HITCH":
                        residualValue *= 0.05m;
                        break;
                    case "COMBUSTION_ENGINE":
                        residualValue *= 0.10m;
                        break;
                    case "DIESEL":
                        residualValue *= 0.04m;
                        break;
                    case "PETROL":
                        residualValue *= 0.07m;
                        break;
                    case "ELECTRIC":
                        residualValue *= 1.05m;
                        break;
                }
            }

            context.LeaseCalculationInformation.VehicleResidualValue = residualValue;
            return Task.FromResult(context);
        }
    }
}
