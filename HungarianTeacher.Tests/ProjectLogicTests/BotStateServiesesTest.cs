using FluentAssertions;
using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Moq;

namespace HungarianTeacher.Tests.ProjectLogicTests

{
    public class BotStateServiesesTest // This class is responsiable for testing BotStateServieses class
    {
        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(14578135, true, true)]
        public async Task SetIsWaitingForMinutesMessageLogic_ReturnExpectedOutput(long chatID, bool isWaitingForMinutesMessage, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface

            // Act - call the method which is tested
            var result = await new BotStateServices(mockDatabase.Object).SetIsWaitingForMinutesMessageLogic(chatID, isWaitingForMinutesMessage); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected); // Assert that the result is what was expected
            mockDatabase.Verify(database => database.SetIsWaitingForMinutesMessage(chatID.ToString(), isWaitingForMinutesMessage), Times.Once); // Verify that the SetIsWaitingForMinutesMessage method was called once with the correct parameters
        }

        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(14578135, true)]
        [InlineData(14578135, false)]
        public async Task GetIsWaitingForMinutesMessageLogic_ReturnExpectedOutput(long chatID, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface
            mockDatabase.Setup(database => database.GetIsWaitingForMinutesMessage(chatID.ToString())).ReturnsAsync(expected); // Mock the database call to return true

            // Act - call the method which is tested
            var result = await new BotStateServices(mockDatabase.Object).GetIsWaitingForMinutesMessageLogic(14578135); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected); // Assert that the result is what was expected
            mockDatabase.Verify(database => database.GetIsWaitingForMinutesMessage(chatID.ToString()), Times.Once); // Verify that the GetIsWaitingForMinutesMessage method was called once with the correct parameters
        }

        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(14578135, true, true)]
        public async Task SetIsWaitingForLanguageMessageLogic_ReturnExpectedOutput(long chatID, bool isWaitingForMinutesMessage, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface

            // Act - call the method which is tested
            var result = await new BotStateServices(mockDatabase.Object).SetIsWaitingForMinutesMessageLogic(chatID, isWaitingForMinutesMessage); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected); // Assert that the result is what was expected
            mockDatabase.Verify(database => database.SetIsWaitingForMinutesMessage(chatID.ToString(), isWaitingForMinutesMessage), Times.Once); // Verify that the SetIsWaitingForMinutesMessage method was called once with the correct parameters

        }

        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(14578135, true)]
        [InlineData(14578135, false)]
        public async Task GetIsWaitingForLanguageMessageLogic_ReturnExpectedOutput(long chatID, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface
            mockDatabase.Setup(database => database.GetIsWaitingForLanguageMessage(chatID.ToString())).ReturnsAsync(expected); // Mock the database call to return true

            // Act - call the method which is tested
            var result = await new BotStateServices(mockDatabase.Object).GetIsWaitingForLanguageMessageLogic(14578135); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected); // Assert that the result is what was expected
            mockDatabase.Verify(database => database.GetIsWaitingForLanguageMessage(chatID.ToString()), Times.Once); // Verify that the GetIsWaitingForMinutesMessage method was called once with the correct parameters
        }
    }
}
