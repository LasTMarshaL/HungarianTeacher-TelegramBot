using HungarianTeacher.Database;
using Serilog;

namespace HungarianTeacher.ProjectLogic


{
    public class BotMessageScheduler 
    {
        private readonly IDatabase _database;

        public BotMessageScheduler(IDatabase database) 
        {
            _database = database;
        }

        public async Task<bool> SetTimeBetweenMessageAndTargetTimeLogic(long chatID, string minutesUsersMessage) 
        {
            if (int.TryParse(minutesUsersMessage, out int minutes)) 
            {
                if (minutes != 0)
                {
                    minutes = Math.Abs(minutes);
                    await _database.SetTimeBetweenMessageAndTargetTime(chatID.ToString(), minutes, DateTime.UtcNow.AddMinutes(minutes).ToUniversalTime().ToString("o"));
                
                    Log.Information("Time between messages was set to {Minutes} minutes for {ChatID}", minutes, chatID);

                    return true; 
                }
                else
                {
                    return false; 
                }
            }
            else
            {
                return false; 
            }
        }

        public async Task<int> GetTimeBetweenMessagesLogic(long chatID) 
        {
            try
            {
                int timeBetweenMessages = await _database.GetTimeBetweenMessages(chatID.ToString()); 
                if (timeBetweenMessages > 0) 
                {
                    return timeBetweenMessages;  
                }
                return 30; 
            }
            catch
            {
                return 30; 
            }
        }

        public async Task<bool> SetTargetTimeLogic(long chatID, int minutes)
        {
            if (minutes > 0)
            {
                await _database.SetTargetTime(chatID.ToString(), DateTime.UtcNow.AddMinutes(minutes).ToUniversalTime().ToString("o")); 
                
                return true; 
            }
            else
            {
                await _database.SetTargetTime(chatID.ToString(), DateTime.UtcNow.AddMinutes(30).ToUniversalTime().ToString("o"));

                Log.Warning($"Warning Failed to set suggested interval for {chatID}, 30-minutes interval was set!"); 

                return false; 
            }
        }

        public async Task<DateTime> GetTargetTimeLogic(long chatID) 
        {
            string targetTimeString = await _database.GetTargetTime(chatID.ToString()); 
            if (DateTime.TryParse(targetTimeString, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime targetTime)) 
            {
                if (targetTime >= DateTime.UtcNow) 
                {
                    return targetTime; 
                }
                else
                {
                   Log.Warning($"Failed to get traget time from the database for {chatID}, new target time = now + 30 minutes!"); 
                    return DateTime.UtcNow.AddMinutes(await GetTimeBetweenMessagesLogic(chatID));
                }  
            }
            else
            {
                Log.Warning($"Failed to get traget time from the database for {chatID}, new target time = now + 30 minutes!"); 
                return DateTime.UtcNow.AddMinutes(await GetTimeBetweenMessagesLogic(chatID)); 
            }
        }
    }
}
