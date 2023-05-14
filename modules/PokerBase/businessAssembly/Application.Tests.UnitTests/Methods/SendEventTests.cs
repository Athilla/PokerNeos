using System;
using System.Threading.Tasks;
using FluentAssertions;
using Moq;
using Moq.AutoMock;
using PokerNeos.Application.Abstractions.Methods;
using PokerNeos.PokerBase.Application.Methods;
using Xunit;
using Xunit.Abstractions;

namespace PokerNeos.PokerBase.Application.Tests.UnitTests.Methods
{
    /// <summary>
    /// Represents SendEventAsync tests.
    /// </summary>
    public class SendEventTests : PokerNeos.Application.XUnit.ApplicationTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendEventAsyncTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public SendEventTests(ITestOutputHelper output)
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
            ISendEvent method = Mocker.CreateInstance<SendEvent>();

            // Act
            System.Action action = () => method.ExecuteAsync(null);

            // Assert
            action.Should().NotThrow();
        }
    }
}