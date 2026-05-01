using FluentAssertions;
using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Moq;

namespace HungarianTeacher.Tests.ProjectLogicTests

{
    public class BotStateServicesTest
    {
        [Theory]
        [InlineData(14578135, true, true)]
        public async Task SetIsWaitingForMinutesMessageLogic_ReturnExpectedOutput(long chatID, bool isWaitingForMinutesMessage, bool expected)
        {
            var mockDatabase = new Mock<IDatabase>();

            var result = await new BotStateServices(mockDatabase.Object).SetIsWaitingForMinutesMessageLogic(chatID, isWaitingForMinutesMessage); 

            result.Should().Be(expected);
            mockDatabase.Verify(database => database.SetIsWaitingForMinutesMessage(chatID.ToString(), isWaitingForMinutesMessage), Times.Once);
        }

        [Theory] 
        [InlineData(14578135, true)]
        [InlineData(14578135, false)]
        public async Task GetIsWaitingForMinutesMessageLogic_ReturnExpectedOutput(long chatID, bool expected)
        {
            var mockDatabase = new Mock<IDatabase>(); 
            mockDatabase.Setup(database => database.GetIsWaitingForMinutesMessage(chatID.ToString())).ReturnsAsync(expected);

            var result = await new BotStateServices(mockDatabase.Object).GetIsWaitingForMinutesMessageLogic(14578135); 

            result.Should().Be(expected);
            mockDatabase.Verify(database => database.GetIsWaitingForMinutesMessage(chatID.ToString()), Times.Once);
        }

        [Theory]
        [InlineData(14578135, true, true)]
        public async Task SetIsWaitingForLanguageMessageLogic_ReturnExpectedOutput(long chatID, bool isWaitingForMinutesMessage, bool expected)
        {
            var mockDatabase = new Mock<IDatabase>(); 

            var result = await new BotStateServices(mockDatabase.Object).SetIsWaitingForMinutesMessageLogic(chatID, isWaitingForMinutesMessage); 

            result.Should().Be(expected);
            mockDatabase.Verify(database => database.SetIsWaitingForMinutesMessage(chatID.ToString(), isWaitingForMinutesMessage), Times.Once); 

        }

        [Theory]
        [InlineData(14578135, true)]
        [InlineData(14578135, false)]
        public async Task GetIsWaitingForLanguageMessageLogic_ReturnExpectedOutput(long chatID, bool expected)
        {
            var mockDatabase = new Mock<IDatabase>(); 
            mockDatabase.Setup(database => database.GetIsWaitingForLanguageMessage(chatID.ToString())).ReturnsAsync(expected);

            var result = await new BotStateServices(mockDatabase.Object).GetIsWaitingForLanguageMessageLogic(14578135); 

            result.Should().Be(expected);
            mockDatabase.Verify(database => database.GetIsWaitingForLanguageMessage(chatID.ToString()), Times.Once);
        }
    }
}
