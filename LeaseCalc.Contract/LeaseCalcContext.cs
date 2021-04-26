using System;

namespace LeaseCalc.Contract
{
    public class LeaseCalcContext
    {
        public string Journey { get; set; }
        public LeaseInformation LeaseInformation { get; set; }
        public CustomerInformation CustomerInformation { get; set; }
        public DriverInformation DriverInformation { get; set; }
        public VehicleInformation VehicleInformation { get; set; }
        public LeaseCalculationInformation LeaseCalculationInformation { get; set; }
        public string[] JourneyBreadcrumbs { get; set; }
    }
}