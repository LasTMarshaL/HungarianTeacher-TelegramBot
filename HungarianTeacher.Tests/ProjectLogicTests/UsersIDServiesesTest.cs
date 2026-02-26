using FluentAssertions;
using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Moq;


namespace HungarianTeacher.Tests.ProjectLogicTests
{
    public class UsersIDServiesesTest
    {
        [Theory] // Theory attribute indicates that this is a parameterized test method
        [InlineData(123456789, true)]
        public async Task AddNewChatIDLogic_ReturnExpectedOutput(long chatID, bool expected)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface

            // Act - call the method which is tested
            var result = await new UsersIDServices(mockDatabase.Object).AddNewChatIDLogic(chatID); // Call the method which is tested

            // Assert - check if the result is what was expected
            result.Should().Be(expected); // Assert that the result is true
            mockDatabase.Verify(database => database.AddNewChatID(chatID.ToString()), Times.Once); // Verify that the AddNewChatID method was called once with the correct parameters
        }

        public static IEnumerable<object[]> GetAllChatIDsLogicTestData()  // Return testdata one by one // IEnumerable loops through the elements one by one
        {
            yield return new object[] { new List<string> { "123456789", "987654321" }, new List<long> { 123456789, 987654321 } }; // yeild return returns the test data one by one
        }

        [Theory] // Theory attribute indicates that this is a parameterized test method
        [MemberData(nameof(GetAllChatIDsLogicTestData))] // Takes data from the GetAllChatIDsLogicTestData pair by pair and runs the test method for each pair of data     
        public async Task GetAllChatIDsLogic_ReturnExpectedOutput(List<string> expectedChatIDsString, List<long> expectedChatIDs)
        {
            // Arrange - variables, classes, etc.
            var mockDatabase = new Mock<IDatabase>(); // Create a "pseudo" database object for the IDatabase interface

            mockDatabase.Setup(database => database.GetAllChatIDs()).ReturnsAsync(expectedChatIDsString); // Mock the database call to return the list of chat IDs as strings
            
            // Act - call the method which is tested
            var result = await new UsersIDServices(mockDatabase.Object).GetAllChatIDsLogic(); // Call the method which is tested
            
            // Assert - check if the result is what was expected
            result.Should().BeEquivalentTo(expectedChatIDs); // Assert that the result is equivalent to the expected list of chat IDs as longs
            mockDatabase.Verify(database => database.GetAllChatIDs(), Times.Once); // Verify that the GetAllChatIDs method was called once with the correct parameters
        }
    }
}
