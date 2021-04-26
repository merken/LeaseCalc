using System;
using System.Threading.Tasks;
using LeaseCalc.Contract;
using Prise.Plugin;

namespace BruteMonthlyLease.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    /// This component calculates the brute monthly leasing fee
    public class BruteMonthlyLease : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var rentalPrice = context.LeaseCalculationInformation.RentalPrice;
            var vehicleResidualValueAfterTerm = context.LeaseCalculationInformation.VehicleResidualValue;
            var amountToLease = rentalPrice - vehicleResidualValueAfterTerm;

            context.LeaseCalculationInformation.MonthlyLeasePrice = amountToLease / context.LeaseInformation.TermInMonths;
            context.LeaseCalculationInformation.MonthlyLeasePrice += 10;

            return Task.FromResult(context);
        }
    }
}
