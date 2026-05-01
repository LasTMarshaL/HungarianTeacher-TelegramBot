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
            var mockDatabase = new Mock<IDatabase>();

            var result = await new UsersIDServices(mockDatabase.Object).AddNewChatIDLogic(chatID);

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
            var mockDatabase = new Mock<IDatabase>();

            mockDatabase.Setup(database => database.GetAllChatIDs()).ReturnsAsync(expectedChatIDsString); 
            
            var result = await new UsersIDServices(mockDatabase.Object).GetAllChatIDsLogic(); 
            
            result.Should().BeEquivalentTo(expectedChatIDs); 
            mockDatabase.Verify(database => database.GetAllChatIDs(), Times.Once); 
        }
    }
}
