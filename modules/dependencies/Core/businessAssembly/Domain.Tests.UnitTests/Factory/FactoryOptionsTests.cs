using DemoMT.AllModules.Application.XUnit;
using FluentAssertions;
using System;
using Transversals.Business.Core.Domain.Factory;
using Transversals.Business.Core.Domain.Factory.Abstractions;
using Transversals.Business.Domain.Entities;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.Factory
{
    public class FactoryOptionsTests : ApplicationTest
    {
        public FactoryOptionsTests(ITestOutputHelper output) : base(output)
        {
        }

        /// <summary>
        /// Savings the should work.
        /// </summary>
        [Fact]
        public void SavingShouldWork()
        {
            Option option5 = new()
            {
                Id = 5,
                Name = "NotExists",
                Value = "1",
                Type = "1"
            };

            IFactoryOptions factoryOptions = this.Mocker.CreateInstance<FactoryOptions>();

            IOptionSavingRules? optionSavingRule5 = factoryOptions.GetOptionSavingRules(option5, Mocker.GetMock<IServiceProvider>().Object);
            optionSavingRule5.Should().BeNull();
        }
    }
}
