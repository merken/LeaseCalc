using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using LeaseCalc.Contract;

namespace LeaseCalc.Insurance.Api.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class InsuranceController : ControllerBase
    {
        private readonly ILogger<InsuranceController> logger;

        public InsuranceController(ILogger<InsuranceController> logger)
        {
            this.logger = logger;
        }

        [HttpGet]
        public string Get() => "Running!";


        [HttpPost]
        public decimal CalculateInsurance(LeaseCalcContext context)
        {
            var valueToInsure = context.VehicleInformation.RetailPrice;
            var insuranceFee = valueToInsure / context.LeaseInformation.TermInMonths;

            if (context.LeaseInformation.TermInMonths <= 12)
                insuranceFee *= 0.02m;
            if (context.LeaseInformation.TermInMonths <= 24)
                insuranceFee *= 0.03m;
            if (context.LeaseInformation.TermInMonths <= 36)
                insuranceFee *= 0.07m;
            if (context.LeaseInformation.TermInMonths <= 48)
                insuranceFee *= 0.11m;
            if (context.LeaseInformation.TermInMonths <= 60)
                insuranceFee *= 0.15m;

            var driverAge = context.DriverInformation.Age;
            if (driverAge <= 21)
                insuranceFee *= 1.25m;
            if (driverAge <= 25)
                insuranceFee *= 1.05m;
            if (driverAge <= 30)
                insuranceFee *= 1.05m;
            if (driverAge <= 65)
                insuranceFee *= 1.10m;
            if (driverAge <= 70)
                insuranceFee *= 1.25m;
            if (driverAge <= 90)
                insuranceFee *= 1.35m;

            return insuranceFee;
        }
    }
}
