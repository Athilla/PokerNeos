using System;
using System.Threading.Tasks;
using FluentAssertions;
using GroupeIsa.Neos.Application.Rules.EventRules;
using Moq;
using Moq.AutoMock;
using PokerNeos.Application.Abstractions.EntityViews;
using PokerNeos.PokerBase.Application.EventRules.ShowGameView;
using Xunit;
using Xunit.Abstractions;

namespace PokerNeos.PokerBase.Application.Tests.UnitTests.EventRules.ShowGameView
{
    /// <summary>
    /// Represents Retrieving tests.
    /// </summary>
    public class RetrievingTests : PokerNeos.Application.XUnit.Rules.RetrievingRuleTest<Retrieving, IShowGameView>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RetrievingTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public RetrievingTests(ITestOutputHelper output)
            : base(output)
        {
        }

        /// <summary>
        /// OnRetrievingAsyncShouldWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task OnRetrievingAsyncShouldWork()
        {
            // Arrange

            // Act
            IRetrievingRuleArguments result = await ExecuteRetrievingRuleAsync(b => b.WithPagination(0, 10));

            // Assert
            result.Items[0].Should().NotBeNull();
        }
    }
}