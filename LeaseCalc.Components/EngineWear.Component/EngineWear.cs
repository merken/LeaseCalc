using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaseCalc.Contract;
using Prise.Plugin;

namespace EngineWear.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component subtracts the engine wear over the course of the lease term
    public class EngineWear : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var estimatedKMsAtEndOfLease = context.LeaseInformation.TermInMonths * (context.LeaseInformation.KMsPerYear / 12);
            decimal wearFactor = 0.0m;

            if (estimatedKMsAtEndOfLease < 50000)
                wearFactor = 0.05m;
            else if (estimatedKMsAtEndOfLease < 80000)
                wearFactor = 0.15m;
            else if (estimatedKMsAtEndOfLease < 120000)
                wearFactor = 0.20m;
            else if (estimatedKMsAtEndOfLease < 160000)
                wearFactor = 0.25m;
            else
                wearFactor = 0.30m;

            context.LeaseCalculationInformation.VehicleResidualValue *= wearFactor;
            return Task.FromResult(context);
        }
    }
}
