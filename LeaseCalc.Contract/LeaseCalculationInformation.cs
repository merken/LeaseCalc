using System;

namespace LeaseCalc.Contract
{
    public class LeaseCalculationInformation
    {
        public decimal RentalPrice { get; set; }
        public decimal VehicleResidualValue { get; set; }
        public decimal MonthlyLeasePrice { get; set; }
    }
}