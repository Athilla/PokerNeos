using GroupeIsa.Neos.Persistence.EntityFramework;
using GroupeIsa.Neos.Shared.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Transversals.Business.Core.Domain.Extensions;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Persistence.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests
{
    public class CoreConfigurationBuilderExtensionsTests : TestBase
    {
        public CoreConfigurationBuilderExtensionsTests(ITestOutputHelper output) : base(output)
        {
        }

        [Fact]
        public void AddOption_BuilderExtensionAddsSingleSetOfServicesWhenCalledTwice()
        {
            ServiceCollection serviceCollection = Mocker.Get<ServiceCollection>();
            serviceCollection.AddScoped<IOptionRepository, OptionRepository>();

            serviceCollection.AddCoreConfiguration(builder => builder.AddOption("tt", true));
            var count = serviceCollection.Count;
            serviceCollection.AddCoreConfiguration(builder => builder.AddOption("tt", true));

            Assert.Equal(count, serviceCollection.Count);
        }

        [Fact]
        public void AddCounter_BuilderExtensionAddsSingleSetOfServicesWhenCalledTwice()
        {
            var serviceCollection = new ServiceCollection();
            NeosPersistenceServices.Configure(serviceCollection);
            serviceCollection.AddScoped<ICounterRepository, CounterRepository>();

            serviceCollection.AddCoreConfiguration(builder => builder.AddCounter("tt", null, null, 1));
            var count = serviceCollection.Count;
            serviceCollection.AddCoreConfiguration(builder => builder.AddCounter("tt", null, null, 1));

            Assert.Equal(count, serviceCollection.Count);
        }
    }
}
