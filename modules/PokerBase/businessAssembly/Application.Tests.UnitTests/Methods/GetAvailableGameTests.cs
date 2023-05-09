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
    /// Represents GetAvailableGame tests.
    /// </summary>
    public class GetAvailableGameTests : PokerNeos.Application.XUnit.ApplicationTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetAvailableGameTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public GetAvailableGameTests(ITestOutputHelper output)
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
            IGetAvailableGame method = Mocker.CreateInstance<GetAvailableGame>();

            // Act
            System.Action action = () => method.ExecuteAsync();

            // Assert
            action.Should().NotThrow();
        }
    }
}