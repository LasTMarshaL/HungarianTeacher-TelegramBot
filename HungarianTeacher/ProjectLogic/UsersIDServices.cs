using HungarianTeacher.Database;

namespace HungarianTeacher.ProjectLogic
{
    public class UsersIDServices 
    {
        private readonly IDatabase _database; 

        public UsersIDServices(IDatabase database) 
        {
            _database = database; 
        }

        public async Task<bool> AddNewChatIDLogic(long chatID) 
        {
            await _database.AddNewChatID(chatID.ToString()); 

            return true; 
        }

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