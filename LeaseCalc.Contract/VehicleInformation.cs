using System;
using System.Collections.Generic;

namespace LeaseCalc.Contract
{
    public class VehicleInformation
    {
        public decimal RetailPrice { get; set; }
        public string Brand { get; set; }
        public string Model { get; set; }

        public IList<VehicleFeature> Features { get; set; }
    }
}