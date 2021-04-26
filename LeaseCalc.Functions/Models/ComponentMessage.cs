using LeaseCalc.Contract;

namespace LeaseCalc.Functions
{
    public class ComponentMessage
    {
        public string ComponentVersion { get; set; }
        public string ComponentName { get; set; }
        public LeaseCalcContext Context { get; set; }
    }
}