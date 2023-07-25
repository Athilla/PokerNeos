using FluentAssertions;
using System;
using System.Linq;
using System.Threading.Tasks;
using Transversals.Business.Core.Domain.OptionEventRules;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Enums;
using Transversals.Business.Domain.Persistence;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.OptionEventRules
{
    /// <summary>
    /// Represents Saving tests.
    /// </summary>
    public class SavingTests : Transversals.Business.Domain.XUnit.Rules.SavingRuleTest<Saving, Option>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SavingTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public SavingTests(ITestOutputHelper output)
            : base(output)
        {
        }

        /// <summary>
        /// OnSavingAsyncShouldWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsyncShouldWork()
        {
            // Arrange
            Option createdItem = new();
            createdItem.Name = OptionNames.DefaultCurrency.ToString();
            createdItem.Value = "1";
            createdItem.Type = "1";

            Currency currency = new()
            {
                Id = 1,
            };
            SetData(currency);

            Mocker.GetMock<ICurrencyRepository>()
                .Setup(c => c.GetQuery())
                .Returns(new[] { currency }.AsQueryable());

            Mocker.GetMock<IServiceProvider>()
                .Setup(s => s.GetService(typeof(ICurrencyRepository)))
                .Returns(Mocker.GetMock<ICurrencyRepository>().Object);

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithCreatedItems(createdItem));

            // Assert
            await func.Should().NotThrowAsync();
        }

        /// <summary>
        /// OnSavingAsync_WithMultiCurrencies_ShouldWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsync_WithMultiCurrencies_ShouldWork()
        {
            // Arrange
            Option createdItem = new();
            createdItem.Name = OptionNames.MultiCurrencies.ToString();
            createdItem.Value = "true";
            createdItem.Type = "false";

            Currency currency = new()
            {
                Id = 1,
            };
            SetData(currency);
            Mocker.GetMock<ICurrencyRepository>()
                           .Setup(c => c.GetQuery())
                           .Returns(new[] { currency }.AsQueryable());

            Mocker.GetMock<IServiceProvider>()
                .Setup(s => s.GetService(typeof(ICurrencyRepository)))
                .Returns(Mocker.GetMock<ICurrencyRepository>().Object);

            Mocker.GetMock<IServiceProvider>()
                .Setup(s => s.GetService(typeof(IOptionRepository)))
                .Returns(Mocker.GetMock<IOptionRepository>().Object);

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithModifiedItems(createdItem));

            // Assert
            await func.Should().NotThrowAsync();
        }

        /// <summary>
        /// OnSavingAsync_WithMultiCurrencies_ShouldWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsync_WithoutSavingRules_ShouldWork()
        {
            // Arrange
            Option createdItem = new();
            createdItem.Name = "";

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithModifiedItems(createdItem));

            // Assert
            await func.Should().NotThrowAsync();
        }
    }
}