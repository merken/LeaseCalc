namespace LeaseCalc.Contract
{
    [System.Serializable]
    public class LeaseCalcException : System.Exception
    {
        public LeaseCalcException() { }
        public LeaseCalcException(string message) : base(message) { }
        public LeaseCalcException(string message, System.Exception inner) : base(message, inner) { }
        protected LeaseCalcException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }
}