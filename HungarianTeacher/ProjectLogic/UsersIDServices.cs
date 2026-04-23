using HungarianTeacher.Database;

namespace HungarianTeacher.ProjectLogic
{
    public class UsersIDServices // This class is responsiable for working with logic of users' Telegram chat IDs.
    {
        private readonly IDatabase _database; // Database instance

        public UsersIDServices(IDatabase database) 
        {
            _database = database; 
        }

        /// <summary>
        /// Adds a new chat identifier to the database.
        /// </summary>
        public async Task<bool> AddNewChatIDLogic(long chatID) 
        {
            await _database.AddNewChatID(chatID.ToString()); 

            return true; 
        }

        /// <summary>
        /// Retrieves all chat IDs from the database and returns them as a list of long integers.
        /// </summary>
        public async Task<List<long>> GetAllChatIDsLogic()
        {
            List<long> allChatIds = new List<long>();

            List<string> allChatIdsString = await _database.GetAllChatIDs(); 

            foreach (string chatIdString in allChatIdsString) 
            {
                if (long.TryParse(chatIdString, out long chatId)) 
                {
                    allChatIds.Add(chatId); 
                }
            }

            return allChatIds;
        }
    }
}