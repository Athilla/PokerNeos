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
    /// Represents GetLastGameInformation tests.
    /// </summary>
    public class GetLastGameInformationTests : PokerNeos.Application.XUnit.ApplicationTest
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="GetLastGameInformationTests"/> class.
        /// </summary>
        /// <param name="output">Output.</param>
        public GetLastGameInformationTests(ITestOutputHelper output)
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
            IGetLastGameInformation method = Mocker.CreateInstance<GetLastGameInformation>();

            // Act
            System.Action action = () => method.ExecuteAsync();

            // Assert
            action.Should().NotThrow();
        }
    }
}