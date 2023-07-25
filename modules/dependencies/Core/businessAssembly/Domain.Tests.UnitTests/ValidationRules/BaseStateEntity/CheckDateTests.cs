using System;
using System.Threading.Tasks;
using FluentAssertions;
using GroupeIsa.Neos.Domain.Rules.ValidationRules;
using Moq;
using Moq.AutoMock;
using Transversals.Business.Core.Domain.BaseStateEntityValidationRules;
using Transversals.Business.Domain.Entities;
using Transversals.Business.Domain.Properties;
using Xunit;
using Xunit.Abstractions;

namespace Transversals.Business.Core.Domain.Tests.UnitTests.BaseStateEntityValidationRules
{
    /// <summary>
    /// Represents CheckDate tests.
    /// </summary>
    public class CheckDateTests : Transversals.Business.Domain.XUnit.Rules.ValidationRuleTest<CheckDate, BaseStateEntity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CheckDateTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public CheckDateTests(ITestOutputHelper output)
            : base(output)
        {
        }

        /// <summary>
        /// ValidationShouldWorkAsync.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task ValidationShouldWorkAsync()
        {
            // Arrange
            BaseStateEntity item = new Company()
            {
                Currency = new Currency(),
                FromDate = null,
                ToDate = null,
            };

            // Act
            IValidationRuleResult result = await ExecuteValidationRuleAsync(item);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Validation_WithValidDates_ShouldWorkAsync()
        {
            // Arrange
            BaseStateEntity item = new Company()
            {
                Currency = new Currency(),
                FromDate = System.DateTime.Now,
                ToDate = System.DateTime.Now.AddMonths(6),
            };

            // Act
            IValidationRuleResult result = await ExecuteValidationRuleAsync(item);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Validation_WithoutToDate_ShouldWorkAsync()
        {
            // Arrange
            BaseStateEntity item = new Company()
            {
                Currency = new Currency(),
                FromDate = System.DateTime.Now,
                ToDate = null,
            };

            // Act
            IValidationRuleResult result = await ExecuteValidationRuleAsync(item);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Validation_WithoutFromDate_ShouldWorkAsync()
        {
            // Arrange
            BaseStateEntity item = new Company()
            {
                Currency = new Currency(),
                FromDate = null,
                ToDate = System.DateTime.Now,
            };

            // Act
            IValidationRuleResult result = await ExecuteValidationRuleAsync(item);

            // Assert
            result.IsSuccess.Should().BeTrue();
        }

        [Fact]
        public async Task Validation_WithFromDateSupToDate_ShouldFaildAsync()
        {
            // Arrange
            BaseStateEntity item = new Company()
            {
                Currency = new Currency(),
                FromDate = System.DateTime.Now.AddDays(5),
                ToDate = System.DateTime.Now,
            };

            // Act
            IValidationRuleResult result = await ExecuteValidationRuleAsync(item);

            // Assert
            result.IsSuccess.Should().BeFalse();
            result.Errors[0].Should().Be(Resources.Core.SavingDateErrorMessage);
        }
    }
}