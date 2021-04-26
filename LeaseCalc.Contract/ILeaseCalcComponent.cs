using System.Threading.Tasks;

namespace LeaseCalc.Contract
{
    public interface ILeaseCalcComponent
    {
        Task<LeaseCalcContext> Calculate(LeaseCalcContext context);
    }
}
