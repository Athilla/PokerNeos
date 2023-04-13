using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using Xunit;
using Xunit.Abstractions;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.PokerBase.Application.Methods;

namespace PokerNeos.PokerBase.Application.Tests.UnitTests.Methods
{
    /// <summary>
    /// Represents BroadcastMessage tests.
    /// </summary>
    public class BroadcastMessageTests : PokerNeos.Application.XUnit.ApplicationTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="BroadcastMessageTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public BroadcastMessageTests(ITestOutputHelper output)
            : base(output)
        {
        }

        /// <summary>
        /// ExecuteAsyncShouldWork.
        /// </summary>
        /// <returns>A <see cref="System.Threading.Tasks.Task"/> representing the asynchronous operation.</returns>
        [Fact]
        public async Task ExecuteAsyncShouldWork()
        {
            // Arrange
            IBroadcastMessage method = Mocker.CreateInstance<BroadcastMessage>();

            // Act
            System.Action action = () => method.ExecuteAsync();

            // Assert
            action.Should().NotThrow();
        }
    }
}