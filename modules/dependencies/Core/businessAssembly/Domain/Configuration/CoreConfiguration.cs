using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Core.Domain.Configuration.Options;

namespace Transversals.Business.Core.Domain.Configuration
{
    public class CoreConfiguration : ICoreConfiguration
    {
        public ICounterService Counters { get; }

        public IOptionService Options { get; }

        public CoreConfiguration(ICounterService counters, IOptionService options)
        {
            Counters = counters;
            Options = options;
        }
    }
}
