using HungarianTeacher.Database;

namespace HungarianTeacher.ProjectLogic
{
    public class UsersIDServices // This class is responsiable for working with logic of users' Telegram chat IDs
    {
        private readonly IDatabase _database; // Database instance

        public UsersIDServices(IDatabase database) // Constructor of this class, which takes the database instance as a parameter
        {
            _database = database; // Assign the database instance to the private field
        }

        public async Task<bool> AddNewChatIDLogic(long chatID) // Add new chat ID to the database
        {
            await _database.AddNewChatID(chatID.ToString()); // Add new chat ID to the database

            return true; // Return true if adding new chat ID was successful
        }

        public async Task<List<long>> GetAllChatIDsLogic() // Get all chat IDs from the database
        {
            List<long> allChatIds = new List<long>(); // Create list for all chat users' Telegram IDs

            List<string> allChatIdsString = await _database.GetAllChatIDs(); // Get all chat IDs from the database as strings

            foreach (string chatIdString in allChatIdsString) // Take users' Telegram chat ID one by one
            {
                if (long.TryParse(chatIdString, out long chatId)) // If user's Telegram chat ID can be converted to long
                {
                    allChatIds.Add(chatId); // Add user's Telegram chat ID to the list
                }
            }

            return allChatIds; // Return list with all users' Telegram chat IDs
        }
    }
}