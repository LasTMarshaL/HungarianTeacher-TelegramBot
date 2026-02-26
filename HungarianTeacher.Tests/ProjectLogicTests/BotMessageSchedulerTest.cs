using FluentAssertions;
using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Moq;

namespace HungarianTeacher.Tests.ProjectLogicTests
{
    public class BotMessageSchedulerTest // This class is responsible for testing the BotMessageScheduler class
    {
        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(123456789, "40", true)]
        [InlineData(123456789, "String", false)]
        [InlineData(123456789, "-10", true)]
        [InlineData(123456789, "0", false)]
        [InlineData(123456789, "S", false)]
        public async Task SetTimeBetweenMessageAndTargetTimeLogic_ReturnExpectedOutput(long chatID, string minutes, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).SetTimeBetweenMessageAndTargetTimeLogic(chatID, minutes); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected);

            //It.IsAny<string>() is used, because target time and time between messages is generated inside the method 
            if (expected) // If the expected result is true (in other cases the method should return false before calling the methods of the database)
            {
                mockDatabase.Verify(database => database.SetTimeBetweenMessageAndTargetTime(chatID.ToString(), It.IsAny<int>(), It.IsAny<string>()), Times.Once); // Verify that the SetIsWaitingForMinutesMessage method was called once with the correct parameters
            }
        }

        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(123456789, 22)]
        [InlineData(123456789, 0)]
        [InlineData(123456789, -5)]
        public async Task GetTimeBetweenMessagesLogic_ReturnExpectedOutput(long chatID, int expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface
            if (expected <= 0)
            {
                expected = 30; // Set expected to 30, as the method should return 30 if the database returns 0
            }

            mockDatabase.Setup(database => database.GetTimeBetweenMessages(chatID.ToString())).ReturnsAsync(expected); // Mock the database call to return expected value

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTimeBetweenMessagesLogic(chatID); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected);
            mockDatabase.Verify(database => database.GetTimeBetweenMessages(chatID.ToString()), Times.Once); // Verify that the SetIsWaitingForMinutesMessage method was called once with the correct parameters
        }

        [Theory] // Theory attribute indicates that this is a parameterized test method

        // Provide test data for the method parameters
        [InlineData(123456789, 20, true)]
        [InlineData(123456789, 0, false)]
        [InlineData(123456789, -10, false)]
        public async Task SetTargetTimeLogic_ReturnExpectedOutput(long chatID, int minutes, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();
           
            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).SetTargetTimeLogic(chatID, minutes); // Mock the database call to set the target time 

            // Assert - check if the result is what was expected
            result.Should().Be(expected);

            // It.IsAny<string>() is used, because target time is generated inside the method 
            mockDatabase.Verify(database => database.SetTargetTime(chatID.ToString(), It.IsAny<string>()), Times.Once); // Verify that the SetIsWaitingForMinutesMessage method was called once with the correct parameters
        }

        [Fact] // Fact attribute indicates that this is a test method that does not take any parameters
        public async Task GetTargetTimeLogic_FutureDate_ReturnValidFutureDate()
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            DateTime futureDate = DateTime.UtcNow.AddHours(1); // Set a future date (1 hour from now)
            mockDatabase.Setup(database => database.GetTargetTime("123456789")).ReturnsAsync(futureDate.ToString("o")); // Mock the database call to return futureDate value // o - international time format

            // Act
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTargetTimeLogic(123456789);  // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(5)); // Assert that the result is what was expected // TimeSpan.FromSeconds(5) is used to allow s expected and actual times
        }

        [Fact] // Fact attribute indicates that this is a test method that does not take any parameters
        public async Task GetTargetTimeLogic_PastDate_ReturnsDefaultFutureDate()
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            DateTime pastDate = DateTime.UtcNow.AddDays(-1); // Set a future date (1 hour from now)
            mockDatabase.Setup(database => database.GetTargetTime("123456789")).ReturnsAsync(pastDate.ToString("o"));// Mock the database call to return pastDate value  // o - international time format
            mockDatabase.Setup(database => database.GetTimeBetweenMessages("123456789")).ReturnsAsync(30); // Mock the database call to return 30 minutes

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTargetTimeLogic(123456789); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5)); // Assert that the result is what was expected // TimeSpan.FromSeconds(5) is used to allow s expected and actual times 
        }

        [Fact] // Fact attribute indicates that this is a test method that does not take any parameters
        public async Task GetTargetTime_InvalidData_ReturnsDefaultFutureDate()
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            mockDatabase.Setup(database => database.GetTargetTime("123456789")).ReturnsAsync("Wrong Data!!!");
            mockDatabase.Setup(database => database.GetTimeBetweenMessages("123456789")).ReturnsAsync(30);

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTargetTimeLogic(123456789);

            // Assert - check if the result is what was expected
            result.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5)); // Assert that the result is what was expected // TimeSpan.FromSeconds(5) is used to allow s expected and actual times
        }
    }
}
