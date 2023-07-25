using System;
using System.Threading.Tasks;
using FluentAssertions;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using Moq;
using Moq.AutoMock;
using Transversals.Business.Core.Domain.CounterEventRules;
using Transversals.Business.Domain.Entities;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.CounterEventRules
{
    /// <summary>
    /// Represents Saving tests.
    /// </summary>
    public class SavingTests : Transversals.Business.Domain.XUnit.Rules.SavingRuleTest<Saving, Counter>
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
        /// OnSavingAsyncShouldNotWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsyncDeletedLockedCounterShouldNotWork()
        {
            // Arrange
            Counter deletedItem = new();
            deletedItem.Locked = true;
            // Act
            ISavingRuleArguments<Counter> result = await ExecuteSavingRuleAsync(b => b.WithDeletedItems(deletedItem));

            // Assert
            result.Cancel.Should().BeTrue();
        }

        /// <summary>
        /// OnSavingAsyncShouldWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsyncDeletedUnlockedCounterShouldWork()
        {
            // Arrange
            Counter deletedItem = new();
            deletedItem.Locked = false;
            // Act
            ISavingRuleArguments<Counter> result = await ExecuteSavingRuleAsync(b => b.WithDeletedItems(deletedItem));

            // Assert
            result.Cancel.Should().BeFalse();
        }
    }
}