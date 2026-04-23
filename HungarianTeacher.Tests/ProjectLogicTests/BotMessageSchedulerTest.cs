using FluentAssertions;
using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Moq;

namespace HungarianTeacher.Tests.ProjectLogicTests
{
    public class BotMessageSchedulerTest
    {
        [Theory] 
        [InlineData(123456789, "40", true)]
        [InlineData(123456789, "String", false)]
        [InlineData(123456789, "-10", true)]
        [InlineData(123456789, "0", false)]
        [InlineData(123456789, "S", false)]
        public async Task SetTimeBetweenMessageAndTargetTimeLogic_ReturnExpectedOutput(long chatID, string minutes, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).SetTimeBetweenMessageAndTargetTimeLogic(chatID, minutes);

            // Assert - check if the result is what was expected
            result.Should().Be(expected);
            if (expected)
            {
                mockDatabase.Verify(database => database.SetTimeBetweenMessageAndTargetTime(chatID.ToString(), It.IsAny<int>(), It.IsAny<string>()), Times.Once); 
            }
        }

        [Theory]
        [InlineData(123456789, 22)]
        [InlineData(123456789, 0)]
        [InlineData(123456789, -5)]
        public async Task GetTimeBetweenMessagesLogic_ReturnExpectedOutput(long chatID, int expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();
            if (expected <= 0)
            {
                expected = 30;
            }
            mockDatabase.Setup(database => database.GetTimeBetweenMessages(chatID.ToString())).ReturnsAsync(expected);

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTimeBetweenMessagesLogic(chatID); 

            // Assert - check if the result is what was expected
            result.Should().Be(expected);
            mockDatabase.Verify(database => database.GetTimeBetweenMessages(chatID.ToString()), Times.Once); 
        }

        [Theory] 
        [InlineData(123456789, 20, true)]
        [InlineData(123456789, 0, false)]
        [InlineData(123456789, -10, false)]
        public async Task SetTargetTimeLogic_ReturnExpectedOutput(long chatID, int minutes, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();
           
            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).SetTargetTimeLogic(chatID, minutes); 

            // Assert - check if the result is what was expected
            result.Should().Be(expected);
            mockDatabase.Verify(database => database.SetTargetTime(chatID.ToString(), It.IsAny<string>()), Times.Once); 
        }

        [Fact]
        public async Task GetTargetTimeLogic_FutureDate_ReturnValidFutureDate()
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            DateTime futureDate = DateTime.UtcNow.AddHours(1);
            mockDatabase.Setup(database => database.GetTargetTime("123456789")).ReturnsAsync(futureDate.ToString("o"));

            // Act
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTargetTimeLogic(123456789);

            // Assert - check if the result is what was expected
            result.Should().BeCloseTo(futureDate, TimeSpan.FromSeconds(5));
        }

        [Fact]
        public async Task GetTargetTmeLogic_PastDate_ReturnsDefaultFutureDate()
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            DateTime pastDate = DateTime.UtcNow.AddDays(-1); 
            mockDatabase.Setup(database => database.GetTargetTime("123456789")).ReturnsAsync(pastDate.ToString("o"));
            mockDatabase.Setup(database => database.GetTimeBetweenMessages("123456789")).ReturnsAsync(30);

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTargetTimeLogic(123456789);

            // Assert - check if the result is what was expected
            result.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5)); 
        }

        [Fact]
        public async Task GetTargetTime_InvalidData_ReturnsDefaultFutureDate()
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            mockDatabase.Setup(database => database.GetTargetTime("123456789")).ReturnsAsync("Wrong Data!!!");
            mockDatabase.Setup(database => database.GetTimeBetweenMessages("123456789")).ReturnsAsync(30);

            // Act - call the method which is tested
            var result = await new BotMessageScheduler(mockDatabase.Object).GetTargetTimeLogic(123456789);

            // Assert - check if the result is what was expected
            result.Should().BeCloseTo(DateTime.UtcNow.AddMinutes(30), TimeSpan.FromSeconds(5));
        }
    }
}
