using HungarianTeacher.Database;
using HungarianTeacher.ProjectLogic;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;


class TelegramBotLogic 
{
    private static TelegramBotHost hungarianStudingBot = new TelegramBotHost("Your token");

    
    private static  List<long> allChatIDs = new List<long>();
    private static IDatabase? _database; 
    private static UsersIDServices? _usersIDServices; 
    private static BotMessageScheduler? _botMessageScheduler;
    private static BotStateServices? _botStateServices;
    private static APIrequest _aPIrequest = new APIrequest();
    private static FileHolder _fileHolder = new FileHolder();

    private static async Task Main() 
    {
        Log.Information("Main logic is lanching"); 

        try
        {
            _database = new Database(); 
            await _database.CreateDatabaseTable();

            Log.Information("Databes was lanched successfuly"); 

            _usersIDServices = new UsersIDServices(_database); 
            _botMessageScheduler = new BotMessageScheduler(_database); 
            _botStateServices = new BotStateServices(_database); 
            _aPIrequest = new APIrequest(); 
            _fileHolder = new FileHolder(); 

            hungarianStudingBot.Start(); 

            allChatIDs = await _usersIDServices.GetAllChatIDsLogic(); 
            foreach (long chatID in allChatIDs) 
            {
                _ = Task.Run(() => CheckTimerLoop(hungarianStudingBot.telegramBotClient, chatID)); 
            }

            hungarianStudingBot.OnMessage += OnMessage;

            Log.Information("Main logic was lanched successfuly"); 

            await Task.Delay(-1);
        }
        catch(Exception ex)
        {
            Log.Error($"Exception: Failed to launch main logic: {ex.Message}!"); 
        }
        finally
        {
            Log.Information("Bot is turning off...");
            Log.CloseAndFlush(); 
        }
    }


    private static async void OnMessage(ITelegramBotClient client, Update update) 
    {
        long chatID = 0; 
        if (update.Message != null) 
        {
            chatID = update.Message.Chat.Id;
        }
        if (update.CallbackQuery?.Message != null) 
        {
            chatID = update.CallbackQuery.Message.Chat.Id; 
        }

        if (chatID != 0) 
        {
            if (!allChatIDs.Contains(chatID)) 
            {
                await _usersIDServices!.AddNewChatIDLogic(chatID);
                allChatIDs.Add(chatID);
                _ = Task.Run(() => CheckTimerLoop(hungarianStudingBot.telegramBotClient, chatID)); 
            }
            if (await _botStateServices!.GetIsWaitingForMinutesMessageLogic(chatID) && update.Message?.Text != null) 
            {
                if (await _botMessageScheduler!.SetTimeBetweenMessageAndTargetTimeLogic(chatID, update.Message.Text))
                {
                    await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); 

                    await client.SendMessage(update.Message.Chat.Id, "Great! Messages will come once  " + await _botMessageScheduler.GetTimeBetweenMessagesLogic(chatID) + " mins."); 
                }
                else 
                {
                    await client.SendMessage(update.Message.Chat.Id, "Incorect input! Try again!"); 
                }
            }
            if (await _botStateServices.GetIsWaitingForLanguageMessageLogic(chatID) && update.Message?.Text != null) 
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); 
                await client.SendMessage(update.Message.Chat.Id, "Translated text: " + _aPIrequest.SendRequestTranslation(update.Message.Text, _aPIrequest.languageCode)); 
            }
            if (update.Message?.Text == "/start") 
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); 
                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); 

                await client.SendMessage(update.Message.Chat.Id, "Welcome! I was created to review Hungarian lessons already completed by my creator. Once every set time, a random rule or set of Hungarian words will appear. Errors are possible, because my creator is a CRAP!"); 
            }
            if (update.Message?.Text == "/set_time") 
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false);
                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); 

                await client.SendMessage(update.Message.Chat.Id, "Great! Specify the number of minutes (an integer) you want to receive messages every. The default is 30 minutes.");

                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, true); 

            }
            if (update.Message?.Text == "/translate")
            {
                await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, false); 
                await _botStateServices.SetIsWaitingForMinutesMessageLogic(chatID, false); 

                var inLineKeyboard = new InlineKeyboardMarkup(new[] 
                {
                new [] 
                {
                    InlineKeyboardButton.WithCallbackData("Hungarian", "callbackHungarian"), 
                    InlineKeyboardButton.WithCallbackData("English", "callbackEnglish"),
                    InlineKeyboardButton.WithCallbackData("Russian", "callbackRussian") 
                }
            });
                await client.SendMessage(update.Message.Chat.Id, "Select language to translate into.", replyMarkup: inLineKeyboard); 
            }
            if (update.Type == UpdateType.CallbackQuery) 
            {
                var callbackQuery = update.CallbackQuery; 
                if (callbackQuery != null) 
                {
                    await client.AnswerCallbackQuery(callbackQuery.Id); 

                    if (callbackQuery.Data == "callbackHungarian" || callbackQuery.Data == "callbackEnglish" || callbackQuery.Data == "callbackRussian") 
                    {
                        await _botStateServices.SetIsWaitingForLanguageMessageLogic(chatID, true); 
                        _aPIrequest.languageCode = callbackQuery.Data.Substring(8, 2).ToLower(); 
                        await client.SendMessage(chatID, "Good! Write your text!"); 
                    }
                }
            }
        }
    }

    private static async Task CheckTimerLoop(ITelegramBotClient client, long chatID)
    {
        await _botMessageScheduler!.SetTargetTimeLogic(chatID, await _botMessageScheduler.GetTimeBetweenMessagesLogic(chatID)); 
        while (true) 
        {
            DateTime targetTime = await _botMessageScheduler.GetTargetTimeLogic(chatID);
            if (DateTime.UtcNow >= targetTime) 
            {
                if (await _botMessageScheduler.SetTargetTimeLogic(chatID, await _botMessageScheduler.GetTimeBetweenMessagesLogic(chatID))) 
                {
                    try
                    {
                        using var thread = File.OpenRead(_fileHolder.GetPictureFile()); 
                        var inputFile = new InputFileStream(thread, Path.GetFileName(_fileHolder.GetPictureFile()));
                        await client.SendPhoto(chatID, inputFile); 
                    }
                    catch (Exception ex) 
                    {
                        Log.Error($"Exception: Failed to send photo to {chatID}: {ex.Message}!"); 
                    }
                }
            }
            await Task.Delay(1000); 
        }
    }
}