using HungarianTeacher.Database;
using Serilog;

namespace HungarianTeacher.ProjectLogic


{
    public class BotMessageScheduler // This class is responsiable for working with logic of time between messages and target time
    {
        private readonly IDatabase _database; // Database instance

        public BotMessageScheduler(IDatabase database) // Constructor of this class, which takes the database instance as a parameter
        {
            _database = database; // Assign the database instance to the private field
        }

        public async Task<bool> SetTimeBetweenMessageAndTargetTimeLogic(long chatID, string minutesUsersMessage) // Set time between messages in the database
        {
            if (int.TryParse(minutesUsersMessage, out int minutes)) // Try to convert user's message to int
            {
                if (minutes != 0) // If minutes is more than 0
                {
                    minutes = Math.Abs(minutes); // Take absolute value of minutes, because time between messages can't be negative
                    await _database.SetTimeBetweenMessageAndTargetTime(chatID.ToString(), minutes, DateTime.UtcNow.AddMinutes(minutes).ToUniversalTime().ToString("o")); // Set time between sending messages
                
                    Log.Information("Time between messages was set to {Minutes} minutes for {ChatID}", minutes, chatID); // Log information about setting time between messages

                    return true; // Return true if conversion was successful
                }
                else
                {
                    return false; // Return true if conversion was failed
                }
            }
            else
            {
                return false; // Return false if conversion was failed 
            }
        }

        public async Task<int> GetTimeBetweenMessagesLogic(long chatID) // Get time between messages from the database
        {
            try
            {
                int timeBetweenMessages = await _database.GetTimeBetweenMessages(chatID.ToString()); // Get time between messages from the database
                if (timeBetweenMessages > 0) // If time between messages is more than 0
                {
                    return timeBetweenMessages;  // Return time between messages
                }
                return 30; // Return default value if getting time between messages failed
            }
            catch
            {
                return 30; // Return default value if getting time between messages failed
            }
        }

        public async Task<bool> SetTargetTimeLogic(long chatID, int minutes) // Set target time in the database
        {
            if (minutes > 0) // If minutes is more then 0
            {
                await _database.SetTargetTime(chatID.ToString(), DateTime.UtcNow.AddMinutes(minutes).ToUniversalTime().ToString("o")); // Set target time in the database // o - international time format
                
                return true; // Return true if setting target time was successful
            }
            else
            {
                await _database.SetTargetTime(chatID.ToString(), DateTime.UtcNow.AddMinutes(30).ToUniversalTime().ToString("o")); // Set target time in the database with defoult value // o - international time format

                Log.Warning($"Warning Failed to set suggested interval for {chatID}, 30-minutes interval was set!"); // Log warning if setting target time was failed

                return false; // Return false if setting target time was failed
            }
        }

        public async Task<DateTime> GetTargetTimeLogic(long chatID) // Get target time from the database
        {
            string targetTimeString = await _database.GetTargetTime(chatID.ToString()); // Get target time from the database
            if (DateTime.TryParse(targetTimeString, null, System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime targetTime)) // Try to convert target time to DateTime
            {
                if (targetTime >= DateTime.UtcNow) // If target time is more then or equal to current time
                {
                    return targetTime; // Return target time if conversion was successful
                }
                else
                {
                   Log.Warning($"Failed to get traget time from the database for {chatID}, new target time = now + 30 minutes!"); // Print warning if target time from the database is less then current time
                    return DateTime.UtcNow.AddMinutes(await GetTimeBetweenMessagesLogic(chatID)); // Return default value of current time with added minutes if conversion was failed 
                }  
            }
            else
            {
                Log.Warning($"Failed to get traget time from the database for {chatID}, new target time = now + 30 minutes!"); // Print warning if conversion of target time from the database was failed
                return DateTime.UtcNow.AddMinutes(await GetTimeBetweenMessagesLogic(chatID)); // Return default value of current time with added minutes if conversion was failed
            }
        }
    }
}
