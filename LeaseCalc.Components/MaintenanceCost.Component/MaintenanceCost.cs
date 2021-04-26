using System;
using System.Linq;
using System.Collections.Generic;
using LeaseCalc.Contract;
using Prise.Plugin;
using System.Threading.Tasks;

namespace MaintenanceCost.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component calculates the monthly maintenance fee
    public class MaintenanceCost : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var estimatedKMsAtEndOfLease = context.LeaseInformation.TermInMonths * (context.LeaseInformation.KMsPerYear / 12);
            decimal estimatedMaintenanceCostPerMonth = 0.0m;

            if (context.VehicleInformation.Features.Any(f => f.FeatureCode == "ELECTRIC"))
                estimatedMaintenanceCostPerMonth = 15m;
            else
            {
                // NOT AN ELECTRIC VEHICLE
                if (estimatedKMsAtEndOfLease < 50000)
                    estimatedMaintenanceCostPerMonth = (1500m / context.LeaseInformation.TermInMonths);
                else if (estimatedKMsAtEndOfLease < 80000)
                    estimatedMaintenanceCostPerMonth = (2400m / context.LeaseInformation.TermInMonths);
                else if (estimatedKMsAtEndOfLease < 120000)
                    estimatedMaintenanceCostPerMonth = (4200m / context.LeaseInformation.TermInMonths);
                else if (estimatedKMsAtEndOfLease < 160000)
                    estimatedMaintenanceCostPerMonth = (8300m / context.LeaseInformation.TermInMonths);
                else
                    estimatedMaintenanceCostPerMonth = (9000m / context.LeaseInformation.TermInMonths);
            }

            context.LeaseCalculationInformation.MonthlyLeasePrice += estimatedMaintenanceCostPerMonth;
            return Task.FromResult(context);
        }
    }
}