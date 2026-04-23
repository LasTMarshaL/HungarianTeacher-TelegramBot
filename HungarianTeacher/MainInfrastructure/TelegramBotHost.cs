using HungarianTeacher.MainInfrastructure;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

public class TelegramBotHost // This class is responsiable for Telegram bot host.
{
    public Action<ITelegramBotClient, Update>? OnMessage;
    public TelegramBotClient telegramBotClient;

    public TelegramBotHost(string _token) 
    {
        telegramBotClient = new TelegramBotClient(_token); 
    }

    /// <summary>
    /// Initializes and starts the bot, configuring it to receive updates from Telegram.
    /// </summary>
    public void Start() 
    {
        var receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = { }
        };
        telegramBotClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions); 

        LoggerConfigurator.Setup();

        Log.Information("Bot was lanched");
    }

    /// <summary>
    /// Handles exceptions that occur during bot operations by logging the error information asynchronously.
    /// </summary>
    private async System.Threading.Tasks.Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        Log.Error($"Exceprion: {exception.Message}"); 
        await System.Threading.Tasks.Task.CompletedTask;
    }

    /// <summary>
    /// Handles an incoming update from the Telegram bot client and triggers the associated message event handler.
    /// </summary>
    private async System.Threading.Tasks.Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        OnMessage?.Invoke(client, update); 
        await System.Threading.Tasks.Task.CompletedTask; 
    }
}
