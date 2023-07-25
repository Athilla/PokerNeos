using DemoMT.AllModules.Persistence;
using FluentAssertions;

using GroupeIsa.Neos.Persistence;
using GroupeIsa.Neos.Persistence.EntityFramework;
using GroupeIsa.Neos.Shared.Metadata;
using GroupeIsa.Neos.Shared.MultiTenant;
using GroupeIsa.Neos.Shared.XUnit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using Moq.AutoMock;
using System;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.Configuration;
using Transversals.Business.Core.Domain.Configuration.Counters;
using Transversals.Business.Core.Domain.Configuration.Options;
using Transversals.Business.Core.Domain.Exceptions;
using Transversals.Business.Core.Domain.Tests.UnitTests.DataBase;
using Transversals.Business.Domain.Persistence;
using Transversals.Business.Persistence.Repositories;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests
{
    public class OptionsTests : TestBase
    {
        private readonly ServiceCollection serviceCollection;
        private readonly IServiceScope serviceScope;

        public OptionsTests(ITestOutputHelper output) : base(output)
        {
            serviceCollection = new ServiceCollection();
            serviceCollection.AddSingleton<IDatabasePersistenceSettings>(new PersistenceSettings
            {
                ConnectionString = string.Empty,
                DatabaseType = NeosDatabaseType.PostgreSQL,
                DefaultConcurrencyAccessMode = DatabaseConcurrencyAccessMode.Disabled

            }); ;
            NeosPersistenceServices.Configure(serviceCollection);
            serviceCollection.AddScoped<IOptionRepository, OptionRepository>();
            
            serviceCollection.AddScoped(s => new Mock<INeosTenantInfoAccessor>().Object);
            serviceCollection.AddScoped(s => new Mock<IUserInfoAccessor>().Object); 
            serviceCollection.AddScoped(s => Mock.Of<IApplicationContext>());

            serviceCollection.AddScoped<INeosDatabaseContext, InMemoryCoreConfigurationContext>();
            serviceCollection.AddSingleton(s => new Mock<IDatabaseExceptionConversionHelper>().Object);
            serviceCollection.AddLogging(configure =>
            {
                configure.AddXunit(output);
            });
            serviceScope = serviceCollection.BuildServiceProvider().CreateScope();
            this.Mocker.Use<IServiceProvider>(serviceCollection.BuildServiceProvider());
        }

        [Fact]
        public async Task OptionsCreateAndGetAsync()
        {
            //Arrange
            IOptionRepository or = serviceScope.ServiceProvider.GetRequiredService<IOptionRepository>();
            var context = serviceScope.ServiceProvider.GetRequiredService<INeosDatabaseContext>();

            Business.Domain.Entities.Option o = new()
            {
                Id = 1,
                Name = "MyOption",
                Value = "1",
                Type = typeof(int).Name

            };
            or.Add(o);
            await context.SaveAsync();

            OptionService options = new OptionService(or, serviceScope.ServiceProvider);
            var automock = CreateMocker();
            var mockCounters = automock.GetMock<ICounterService>();

            ICoreConfiguration conf = new CoreConfiguration(mockCounters.Object, options);

            var tt = conf.Options["MyOption"];

            tt.Name.Should().Equals(o.Name);
            tt.Value.Should().Equals(o.Value);
        }

        [Fact]
        public async Task OptionCreateAndGetAndNewValueAsyncAsync()
        {
            //Arrange
            var automock = CreateMocker();
            var mockCounters = automock.GetMock<ICounterService>();

            IOptionRepository or = serviceScope.ServiceProvider.GetRequiredService<IOptionRepository>();
            var context = serviceScope.ServiceProvider.GetRequiredService<INeosDatabaseContext>();

            Business.Domain.Entities.Option o = new()
            {
                Id = 1,
                Name = "MyOption",
                Value = "1",
                Type = typeof(int).Name

            };
            or.Add(o);
            await context.SaveAsync();

            OptionService options = new OptionService(or, serviceScope.ServiceProvider);
            ICoreConfiguration conf = new CoreConfiguration(mockCounters.Object, options);

            var tt = conf.Options["MyOption"];

            tt.Name.Should().Equals(o.Name);
            tt.Value.Should().Equals(o.Value);

            await tt.UpdateValueAsync(10.ToString());

            Business.Domain.Entities.Option? opt = await or.FindAsync(o.Id, o.Name);

            opt.Should().NotBeNull();
            opt?.Value.Should().Be(o.Value);
            tt.GetValue<int>().Should().Be(10);
        }

        [Fact]
        public async Task OptionCreateNewAndGetValueAndUpdateValueAsyncAsync()
        {
            //Arrange
            var automock = CreateMocker();
            var mockCounters = automock.GetMock<ICounterService>();
            IOptionRepository or = serviceScope.ServiceProvider.GetRequiredService<IOptionRepository>();

            OptionService options = new OptionService(or, serviceScope.ServiceProvider);
            ICoreConfiguration conf = new CoreConfiguration(mockCounters.Object, options);

            await conf.Options.CreateNewOptionAsync("MyNewOption", true);

            var tt = conf.Options["MyNewOption"];

            tt.Name.Should().Equals("MyNewOption");
            tt.Value.Should().Equals(true.ToString());

            var opt = await or.FindAsync(tt.Id, tt.Name);

            opt.Should().NotBeNull();
            opt?.Value.Should().Be(tt.Value);
            opt?.Type.Should().Be(tt.Type);

            tt.GetValue<bool>().Should().BeTrue();
        }


        [Fact]
        public void FindOptionNotExistThrowAsync()
        {
            //Arrange
            var automock = CreateMocker();
            var mockCounters = automock.GetMock<ICounterService>();
            IOptionRepository or = serviceScope.ServiceProvider.GetRequiredService<IOptionRepository>();

            OptionService options = new OptionService(or, serviceScope.ServiceProvider);
            ICoreConfiguration conf = new CoreConfiguration(mockCounters.Object, options);

            Action act = () =>
            {
                _ = conf.Options["MyNewOption"];
            };

            act.Should().ThrowExactly<NullValueException>();

        }


        [Fact]
        public async Task UpdateNullValueAsync()
        {
            //Arrange
            var automock = new AutoMocker();

            var mockCounters = automock.GetMock<ICounterService>();
            var mockoptions = automock.GetMock<IOptionService>();

            Option option = new Option(new Business.Domain.Entities.Option
            {
                Name = "AZERTY",
                Value = "",
                Type = typeof(int).Name
            }, serviceScope.ServiceProvider);

            mockoptions.Setup(o => o[It.IsAny<string>()]).Returns(option);
            var conf = new CoreConfiguration(mockCounters.Object, mockoptions.Object);

            var tt = conf.Options["aa"];
            var result = await tt.UpdateValueAsync(tt.Value);
            result.Should().BeFalse();
        }

        [Fact]
        public void TryFindOptionNotExistReturnFalse()
        {
            //Arrange
            IOptionRepository or = serviceScope.ServiceProvider.GetRequiredService<IOptionRepository>();

            OptionService options = new OptionService(or, serviceScope.ServiceProvider);

            var result = options.TryFindOption("MyNewOption");
            result.Should().BeFalse();
        }
        [Fact]
        public async Task TryFindOptionExistReturnTrueAsync()
        {
            //Arrange
            IOptionRepository or = serviceScope.ServiceProvider.GetRequiredService<IOptionRepository>();
            OptionService options = new OptionService(or, serviceScope.ServiceProvider);

            await options.CreateNewOptionAsync("MyNewOption", true);
            var result = options.TryFindOption("MyNewOption");
            result.Should().BeTrue();
        }
    }
}
