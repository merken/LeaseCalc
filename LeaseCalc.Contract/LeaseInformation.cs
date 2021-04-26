using System;

namespace LeaseCalc.Contract
{
    public class LeaseInformation
    {
        public int TermInMonths { get; set; }
        public decimal InitialDownPayment { get; set; }
        public int KMsPerYear { get; set; }
    }
}