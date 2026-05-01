using HungarianTeacher.Database;
using Serilog;

namespace HungarianTeacher.ProjectLogic
{
    public class BotStateServices 
    {
        private readonly IDatabase _database;

        public BotStateServices(IDatabase database) 
        {
                _database = database; 
        }

        public async Task<bool> SetIsWaitingForLanguageMessageLogic(long chatID, bool newFlag) 
        {
            await _database.SetIsWaitingForLanguageMessage(chatID.ToString(), newFlag);

            return true; 
        }

        public async Task<bool> GetIsWaitingForLanguageMessageLogic(long chatID)
        {
            bool currentFlag = await _database.GetIsWaitingForLanguageMessage(chatID.ToString());

            return currentFlag;
        }

        public async Task<bool> SetIsWaitingForMinutesMessageLogic(long chatID, bool newFlag) 
        {
            await _database.SetIsWaitingForMinutesMessage(chatID.ToString(), newFlag); 

            return true; 
        }

        public async Task<bool> GetIsWaitingForMinutesMessageLogic(long chatID) 
        {
            bool currentFlag = await _database.GetIsWaitingForMinutesMessage(chatID.ToString());

            return currentFlag;
        }
    }
}
