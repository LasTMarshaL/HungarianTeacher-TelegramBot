using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


class TelegramBotLogic // This class is responsiable for logic
{
    private static TelegramBotHost hungarianStudingBot = new TelegramBotHost("8312704065:AAGWs8Yz4Sz8o3VR1AqwC7hlkU4MtOuh2NI"); // Create instance with token

    
    private static  List<long> allChatIDs = new List<long>(); // Thsi list is used to check if user's Teelegram chat ID is already added to database
    private static IDatabase? _database; // instance of Database class
    private static UsersIDServices? _usersIDServices; // Instance of UsersIDServieses class
    private static BotMessageScheduler? _botMessageScheduler; // Instance of BotMessageScheduler class
    private static BotStateServices? _botStateServices; // Instance of BotStateServieses class
    private static APIrequest _aPIrequest = new APIrequest(); // Instance of APIrequest class
    private static FileHolder _fileHolder = new FileHolder(); // Instance of DataBase class

    private static async Task Main() // Main method
    {
        Log.Information("Main logic is lanching"); // Log information about the lanching main logic

        try
        {
            _database = new Database(); // Injection
            await _database.CreateDatabaseTable(); // Create table in the database if it doesn't exist

            Log.Information("Databes was lanched successfuly"); // Log information about the lanching database

            _usersIDServices = new UsersIDServices(_database); // Injection
            _botMessageScheduler = new BotMessageScheduler(_database); // Injection
            _botStateServices = new BotStateServices(_database); // Injection
            _aPIrequest = new APIrequest(); // Injection
            _fileHolder = new FileHolder(); // Injection

            hungarianStudingBot.Start(); // Lanching bot

            allChatIDs = await _usersIDServices.GetAllChatIDsLogic(); // Get all Telegram chat IDs
            foreach (long chatID in allChatIDs) // Take users' Telegram chat ID one by one
            {
                _ = Task.Run(() => CheckTimerLoop(hungarianStudingBot.telegramBotClient, chatID)); // Lanch async task for time loop // _ means, that We don't have to wait for its finishing
            }

            hungarianStudingBot.OnMessage += OnMessage; // Method withc is used after getting a message

            Log.Information("Main logic was lanched successfuly"); // Log information about the lanching main logic

            await Task.Delay(-1); // Make the main method never end to keep the bot working
        }
        catch(Exception ex)
        {
            Log.Error($"Exception: Failed to launch main logic: {ex.Message}!"); // Log the exception if lanching main logic failed
        }
        finally
        {
            Log.Information("Bot is turning off..."); // Log information about the lanching main logic
            Log.CloseAndFlush(); // Close and flush the log to make sure all log messages are written to the log file
        }
    }

    private static async void OnMessage(ITelegramBotClient client, Update update) // Method which is used to work with user's messages
    {
        long chatID = 0; // User's chat Id who sent a message
        if (update.Message != null) // If new update is a message 
        {
            chatID = update.Message.Chat.Id; // Take Telegram chat ID from the message
        }
        if (update.CallbackQuery?.Message != null) // If new update is a pressed button
        {
            chatID = update.CallbackQuery.Message.Chat.Id; // Take Telegram chat ID from the pressed button
        }

        if (chatID != 0) // If Telegram chat ID exist
        {
            if (!allChatIDs.Contains(chatID)) // Chexk if user's Telegram chat ID is already added to the database or not using that lsit
            {
                await _usersIDServices!.AddNewChatIDLogic(chatID);
                allChatIDs.Add(chatID);
                _ = Task.Run(() => CheckTimerLoop(hungarianStudingBot.telegramBotClient, chatID)); // Lanch async task for time loop. _ is needed to show, that We don't have to wait for its finishing
            }
            if (await _botStateServices!.GetIsWaitingForMinutesMessageLogic(chatID) && update.Message?.Text != null) // If bot is waiting user's message
            {
                if (await _botMessageScheduler!.SetTimeBetweenMessageAndTargetTimeLogic(chatID, update.Message.Text))
                {
                    await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); // Now not ended waiting for user's message

                    await client.SendMessage(update.Message.Chat.Id, "Great! Messages will come once  " + await _botMessageScheduler.GetTimeBetweenMessagesLogic(chatID) + " mins."); // Send a message, that time is set
                }
                else // In case of exception
                {
                    await client.SendMessage(update.Message.Chat.Id, "Incorect input! Try again!"); // Send a message, that input is incorect
                }
            }
            if (await _botStateServices.GetIsWaitingForLanguageMessageLogic(chatID) && update.Message?.Text != null) // If bot is waiting user's message
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); // Now not ended waiting for user's message
                await client.SendMessage(update.Message.Chat.Id, "Translated text: " + _aPIrequest.SendRequestTranslation(update.Message.Text, _aPIrequest.languageCode)); // Send translated text
            }
            if (update.Message?.Text == "/start") // If message = "/start"
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); // Now not ended waiting for user's message
                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); // Now not ended waiting for user's message

                await client.SendMessage(update.Message.Chat.Id, "Welcome! I was created to review Hungarian lessons already completed by my creator. Once every set time, a random rule or set of Hungarian words will appear. Errors are possible, because my creator is a CRAP!"); // Send the first message
            }
            if (update.Message?.Text == "/set_time") // If message = /set_time"
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); // Now not ended waiting for user's message
                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); // Now not ended waiting for user's messagege

                await client.SendMessage(update.Message.Chat.Id, "Great! Specify the number of minutes (an integer) you want to receive messages every. The default is 30 minutes."); // Ask user to set time

                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, true); // Bot started waiting for user's answer

            }
            if (update.Message?.Text == "/translate") // If message = "/translate"
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); // Now not ended waiting for user's message
                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); // Now not ended waiting for user's messagege

                var inLineKeyboard = new InlineKeyboardMarkup(new[] // New buttons under bot's message
                {
                new [] //   New buttons
                {
                    InlineKeyboardButton.WithCallbackData("Hungarian", "callbackHungarian"), // Button to translate into hungarian
                    InlineKeyboardButton.WithCallbackData("English", "callbackEnglish"), // Button to translate into english
                    InlineKeyboardButton.WithCallbackData("Russian", "callbackRussian") // Button to translate into russian
                }
            });
                await client.SendMessage(update.Message.Chat.Id, "Select language to translate into.", replyMarkup: inLineKeyboard); // Send a message  to ask user's send his text
            }
            if (update.Type == UpdateType.CallbackQuery) // If button was pressed
            {
                var callbackQuery = update.CallbackQuery; // Data of pressed button
                if (callbackQuery != null) // Data of pressed button exists
                {
                    await client.AnswerCallbackQuery(callbackQuery.Id); // Start working with this data

                    if (callbackQuery.Data == "callbackHungarian" || callbackQuery.Data == "callbackEnglish" || callbackQuery.Data == "callbackRussian") // If data of pressed button = "*"
                    {
                        await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, true); // Now bot started waiting for user's message
                        _aPIrequest.languageCode = callbackQuery.Data.Substring(8, 2).ToLower(); // Get language code from data of pressed button 
                        await client.SendMessage(chatID, "Good! Write your text!"); // Send a message to ask user to send his text
                    }
                }
            }
        }
    }

    private static async Task CheckTimerLoop(ITelegramBotClient client, long chatID) // Time loop
    {
        await _botMessageScheduler!.SetTargetTimeLogic(chatID, await _botMessageScheduler.GetTimeBetweenMessagesLogic(chatID)); // Set traget time from the database for user 
        while (true) // Loop
        {
            DateTime targetTime = await _botMessageScheduler.GetTargetTimeLogic(chatID); // Get target time from the database for user
            if (DateTime.UtcNow >= targetTime) // If current time > or = target time
            {
                if (await _botMessageScheduler.SetTargetTimeLogic(chatID, await _botMessageScheduler.GetTimeBetweenMessagesLogic(chatID))) // If traget time from the database for user again is set corectly
                {
                    try
                    {
                        using var thread = File.OpenRead(_fileHolder.GetPictureFile()); // Open file with picture for sending
                        var inputFile = new InputFileStream(thread, Path.GetFileName(_fileHolder.GetPictureFile())); // Create InputFileStream for sending picture
                        await client.SendPhoto(chatID, inputFile); // Send picture to the user
                    }
                    catch (Exception ex) 
                    {
                        Log.Error($"Exception: Failed to send photo to {chatID}: {ex.Message}!"); // Log the exception if sending photo failed
                    }
                }
            }
            await Task.Delay(1000); // Delay to not have much preasure on CPU
        }
    }
}