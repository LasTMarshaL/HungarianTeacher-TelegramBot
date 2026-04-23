using HungarianTeacher.Database;
using Serilog;

namespace HungarianTeacher.ProjectLogic
{
    public class BotStateServices // This class is responsiable for working with logic of bot state (waiting for language message, waiting for minutes message).
    {
        private readonly IDatabase _database;

        public BotStateServices(IDatabase database) 
        {
                _database = database; 
        }

        /// <summary>
        /// Sets the waiting status for language message processing for the specified chat.
        /// </summary>
        public async Task<bool> SetIsWaitingForLanguageMessageLogic(long chatID, bool newFlag) 
        {
            await _database.SetIsWaitingForLanguageMessage(chatID.ToString(), newFlag);

            return true; 
        }

        /// <summary>
        /// ADetermines whether the specified chat is currently awaiting a language message.
        /// </summary>
        public async Task<bool> GetIsWaitingForLanguageMessageLogic(long chatID)
        {
            bool currentFlag = await _database.GetIsWaitingForLanguageMessage(chatID.ToString());

            return currentFlag;
        }

        /// <summary>
        /// Sets the waiting state for minutes message logic for the specified chat.
        /// </summary>
        public async Task<bool> SetIsWaitingForMinutesMessageLogic(long chatID, bool newFlag) 
        {
            await _database.SetIsWaitingForMinutesMessage(chatID.ToString(), newFlag); 

            return true; 
        }

        /// <summary>
        /// Determines whether the specified chat is currently awaiting a minutes message.
        /// </summary>
        public async Task<bool> GetIsWaitingForMinutesMessageLogic(long chatID) 
        {
            bool currentFlag = await _database.GetIsWaitingForMinutesMessage(chatID.ToString());

            return currentFlag;
        }
    }
}
