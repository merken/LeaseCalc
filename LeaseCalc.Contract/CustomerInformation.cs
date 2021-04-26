using System;

namespace LeaseCalc.Contract
{
    public class CustomerInformation
    {
        public int CustomerId { get; set; }
        public int CustomerCreditScore { get; set; }
        public string EmailAddress { get; set; }
    }
}