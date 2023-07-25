using System;
using System.Threading.Tasks;
using FluentAssertions;
using GroupeIsa.Neos.Domain.Exceptions;
using GroupeIsa.Neos.Domain.Rules.EventRules;
using Moq;
using Moq.AutoMock;
using Transversals.Business.Core.Domain.BaseStateEntityEventRules;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Properties;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.BaseStateEntityEventRules
{
    /// <summary>
    /// Represents Saving tests.
    /// </summary>
    public class SavingTests : Transversals.Business.Domain.XUnit.Rules.SavingRuleTest<Saving, BaseStateEntity>
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
            MyCompany createdItem = new();
            createdItem.FromDate = new DateTime(2015, 12, 31);
            createdItem.ToDate = DateTime.Today.Date;

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithCreatedItems(createdItem));

            // Assert
            await func.Should().NotThrowAsync();
        }

        /// <summary>
        /// OnSavingAsyncShouldWorkWithNull.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsyncShouldWorkWithNull()
        {
            // Arrange
            MyCompany createdItem = new();
            createdItem.FromDate = null;
            createdItem.ToDate = null;

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithCreatedItems(createdItem));

            // Assert
            await func.Should().NotThrowAsync();
        }

        /// <summary>
        /// OnSavingAsyncShouldWorkWithSameDate.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsyncShouldWorkWithSameDate()
        {
            // Arrange
            MyCompany createdItem = new();
            createdItem.FromDate = DateTime.Today.Date;
            createdItem.ToDate = createdItem.FromDate;

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithCreatedItems(createdItem));

            // Assert
            await func.Should().NotThrowAsync();
        }

        /// <summary>
        /// OnSavingAsyncShouldNotWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnSavingAsyncShouldNotWork()
        {
            // Arrange
            MyCompany createdItem = new();
            createdItem.FromDate = DateTime.Today.Date;
            createdItem.ToDate = new DateTime(2015, 12, 31);

            // Act
            Func<Task> func = () => ExecuteSavingRuleAsync(b => b.WithCreatedItems(createdItem));

            // Assert
            await func.Should().ThrowExactlyAsync<BusinessException>().WithMessage(Resources.Core.SavingDateErrorMessage); ;

        }

        private class MyCompany : BaseStateEntity
        {

        }
    }
}