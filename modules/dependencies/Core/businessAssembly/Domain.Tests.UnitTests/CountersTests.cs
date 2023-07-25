using FluentAssertions;
using GroupeIsa.Neos.Application.MultiTenant;
using GroupeIsa.Neos.Persistence.EntityFramework;
using GroupeIsa.Neos.Shared.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System.Linq;
using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Persistence.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests
{
    public class CountersTests : TestBase
    {
        private readonly ServiceCollection serviceCollection;
        private readonly IServiceScope serviceScope;

        public CountersTests(ITestOutputHelper output) : base(output)
        {
            serviceCollection = new ServiceCollection();
            NeosPersistenceServices.Configure(serviceCollection);
            serviceCollection.AddScoped(s => new Mock<ITenants>().Object);
            serviceCollection.AddScoped<ICounterRepository, CounterRepository>();
            serviceCollection.AddLogging(configure =>
            {
                configure.AddXunit(output);
            });
            serviceScope = serviceCollection.BuildServiceProvider().CreateScope();

        }

        [Fact]
        public void TryFindCounterExistReturnTrue()
        {
            //Arrange
            var automock = this.Mocker;
            int initValue = 1;
            Business.Domain.Entities.Counter c = new()
            {
                Id = 1,
                Name = "MyCounter",
                Value = initValue,
                Prefixe = "",
                Suffixe = ""
            };
            var mockRepo = automock.GetMock<ICounterRepository>();
            mockRepo.Setup(x => x.GetQuery()).Returns(new[] { c }.AsQueryable()).Verifiable();
            using var sp = serviceCollection.BuildServiceProvider();
            ICounterService counterService = automock.CreateInstance<CounterService>();
            var result = counterService.TryFindCounter("MyCounter");
            result.Should().BeTrue();
        }

        [Theory]
        [InlineData(3, "", "", 3)]
        [InlineData(0, "", "", null)]
        [InlineData(-1, "P", "S", 1)]
        public void TryFindCounterNotExistReturnFalseAsync(int? value, string prefixe, string suffixe, int? maxValue)
        {
            //Arrange
            var automock = Mocker;
            NeosPersistenceServices.Configure(serviceCollection);
            Business.Domain.Entities.Counter c = new()
            {
                Id = 1,
                Name = "MyCounter",
                Value = value,
                Prefixe = prefixe,
                Suffixe = suffixe,
                MaxValue = maxValue
            };
            var mockRepo = automock.GetMock<ICounterRepository>();
            mockRepo.Setup(x => x.GetQuery()).Returns(new[] { c }.AsQueryable()).Verifiable();

            serviceCollection.AddScoped(provider => mockRepo.Object);

            using var sp = serviceCollection.BuildServiceProvider();
            //automock.Use<IServiceProvider>(sp);

            ICounterService counterService = automock.CreateInstance<CounterService>();
            var result = counterService.TryFindCounter("MyCounter2");
            result.Should().BeFalse();
        }

    }
}
