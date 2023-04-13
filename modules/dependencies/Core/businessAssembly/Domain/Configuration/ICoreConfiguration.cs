using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Core.Domain.Configuration.Options;

namespace Transversals.Business.Core.Domain.Configuration
{
    public interface ICoreConfiguration
    {
        ICounterService Counters { get; }
        IOptionService Options { get; }
    }
}
