using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using LeaseCalc.Contract;
using Prise.Plugin;

namespace CustomerRisk.Component
{
    [Plugin(PluginType = typeof(ILeaseCalcComponent))]
    // This component calculates he risk for a customer based on its credit score
    public class CustomerRisk : ILeaseCalcComponent
    {
        public Task<LeaseCalcContext> Calculate(LeaseCalcContext context)
        {
            var riskFactor = 0.0m;
            
            if (context.CustomerInformation.CustomerCreditScore < 10)
                riskFactor = 0.40m;
            else if (context.CustomerInformation.CustomerCreditScore < 50)
                riskFactor = 0.30m;
            else if (context.CustomerInformation.CustomerCreditScore < 75)
                riskFactor = 0.20m;
            else if (context.CustomerInformation.CustomerCreditScore < 90)
                riskFactor = 0.10m;
            else
                riskFactor = 0.05m;

            context.LeaseCalculationInformation.MonthlyLeasePrice *= riskFactor;

            return Task.FromResult(context);
        }
    }
}