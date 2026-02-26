using HungarianTeacher.Database;
using Serilog;

namespace HungarianTeacher.ProjectLogic
{
    public class BotStateServices // This class is responsiable for working with logic of bot state (waiting for language message, waiting for minutes message)
    {
        private readonly IDatabase _database; // Database instance

        public BotStateServices(IDatabase database) // Constructor of this class, which takes the database instance as a parameter
        {
                _database = database; // Assign the database instance to the private field
        }

        public async Task<bool> SetIsWaitingForLanguageMessageLogic(long chatID, bool newFlag) // Set flag of waiting for language message in the database
        {
            await _database.SetIsWaitingForLanguageMessage(chatID.ToString(), newFlag); // Set flag of waiting for language message in the database

            return true; // Return true if setting flag of waiting for language message was successful
        }

        public async Task<bool> GetIsWaitingForLanguageMessageLogic(long chatID) // Get flag of waiting for language message from the database
        {
            bool currentFlag = await _database.GetIsWaitingForLanguageMessage(chatID.ToString());

            return currentFlag; // Return current flag of waiting for language message
        }

        public async Task<bool> SetIsWaitingForMinutesMessageLogic(long chatID, bool newFlag) // Set flag of waiting for minutes message in the database
        {
            await _database.SetIsWaitingForMinutesMessage(chatID.ToString(), newFlag); // Set flag of waiting for minutes message in the database

            return true; // Return true if setting flag of waiting for minutes message was successful
        }

        public async Task<bool> GetIsWaitingForMinutesMessageLogic(long chatID) // Get flag of waiting for minutes message from the database
        {
            bool currentFlag = await _database.GetIsWaitingForMinutesMessage(chatID.ToString()); // Get flag of waiting for minutes message from the database

            return currentFlag; // Return current flag of waiting for minutes message
        }
    }
}
