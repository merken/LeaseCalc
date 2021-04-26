using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaseCalc.Contract;
using Prise.Plugin;

namespace Day1.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component subtracts 25% of the price of the vehicle after day 1
    public class Day1 : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            context.LeaseCalculationInformation.VehicleResidualValue = context.LeaseCalculationInformation.RentalPrice *  0.75m;
            return Task.FromResult(context);
        }
    }
}
