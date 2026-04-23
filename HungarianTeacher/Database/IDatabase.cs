
namespace HungarianTeacher.Database
{
    public interface IDatabase // Interface for working with the databases and tests.
    {
        Task AddNewChatID(string chatID);
        Task<List<string>> GetAllChatIDs();
        Task SetIsWaitingForLanguageMessage(string chatID, bool newFlag);
        Task<bool> GetIsWaitingForLanguageMessage(string chatID);
        Task SetIsWaitingForMinutesMessage(string chatID, bool newFlag);
        Task<bool> GetIsWaitingForMinutesMessage(string chatID);
        Task SetTimeBetweenMessageAndTargetTime(string chatID, int minutes, string targetTime);
        Task<int> GetTimeBetweenMessages(string chatID);
        Task SetTargetTime(string chatID, string targetTime);
        Task<string> GetTargetTime(string chatID);
        Task CreateDatabaseTable();
    }
}
