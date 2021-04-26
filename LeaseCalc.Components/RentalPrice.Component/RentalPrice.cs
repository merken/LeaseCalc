using System;
using System.Linq;
using System.Collections.Generic;
using LeaseCalc.Contract;
using Prise.Plugin;
using System.Threading.Tasks;

namespace RentalPrice.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component rental price of the lease
    public class RentalPrice : ILeaseCalcComponent
    {
        public int OrderOfCalculation => 1;
        
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            context.LeaseCalculationInformation = new LeaseCalculationInformation();
            context.LeaseCalculationInformation.RentalPrice = context.VehicleInformation.RetailPrice - context.LeaseInformation.InitialDownPayment;
            return Task.FromResult(context);
        }
    }
}
