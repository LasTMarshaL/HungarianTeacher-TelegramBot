using FluentAssertions;
using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Moq;


namespace HungarianTeacher.Tests.ProjectLogicTests
{
    public class UsersIDServiesesTest
    {
        [Theory]
        [InlineData(123456789, true)]
        public async Task AddNewChatIDLogic_ReturnExpectedOutput(long chatID, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            // Act - call the method which is tested
            var result = await new UsersIDServices(mockDatabase.Object).AddNewChatIDLogic(chatID);

            // Assert - check if the result is what was expected
            result.Should().Be(expected);
            mockDatabase.Verify(database => database.AddNewChatID(chatID.ToString()), Times.Once);
        }

        public static IEnumerable<object[]> GetAllChatIDsLogicTestData()
        {
            yield return new object[] { new List<string> { "123456789", "987654321" }, new List<long> { 123456789, 987654321 } }; 
        }

        [Theory] 
        [MemberData(nameof(GetAllChatIDsLogicTestData))] 
        public async Task GetAllChatIDsLogic_ReturnExpectedOutput(List<string> expectedChatIDsString, List<long> expectedChatIDs)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>();

            mockDatabase.Setup(database => database.GetAllChatIDs()).ReturnsAsync(expectedChatIDsString); 
            
            // Act - call the method which is tested
            var result = await new UsersIDServices(mockDatabase.Object).GetAllChatIDsLogic(); 
            
            // Assert - check if the result is what was expected
            result.Should().BeEquivalentTo(expectedChatIDs); 
            mockDatabase.Verify(database => database.GetAllChatIDs(), Times.Once); 
        }
    }
}
