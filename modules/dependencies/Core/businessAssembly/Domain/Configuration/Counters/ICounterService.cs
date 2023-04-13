using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Transversals.Business.Core.Domain.Configuration.Counters
{
    public interface ICounterService
    {
        [ExcludeFromCodeCoverage]
        Task CreateNewCounterAsync(string name, string? prefix, string? suffix, int initialValue, int maxValue = int.MaxValue - 1);
        Task<string> NextFormatedValueAsync(string counterName);
        bool TryFindCounter(string counterName);
    }
}