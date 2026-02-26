using HungarianTeacher.MainInfrastructure;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

public class TelegramBotHost // This class is responsiable for host
{
    public Action<ITelegramBotClient, Update>? OnMessage;// Define method OnMessage // Action allows use methodd as parametres // ITelegramBotClient - bot client to send messages
    public TelegramBotClient telegramBotClient; // Bot client

    public TelegramBotHost(string _token) // Create instance with token
    {
        telegramBotClient = new TelegramBotClient(_token); // Create bot client using token
    }

    public void Start() // Start bot
    {
        var receiverOptions = new ReceiverOptions 
        {
            AllowedUpdates = { } // Get all updates (messages, commands, etc.)
        };
        telegramBotClient.StartReceiving(UpdateHandler, ErrorHandler, receiverOptions); // Start geting messages 

        LoggerConfigurator.Setup(); // Configure Serilog logger

        Log.Information("Bot was lanched"); // Log information about the lanching bot
    }

    // Async method can be implemented wiht other method  at the same time
    // Task means, that program can start this method and do other actions during working of the method
    private async System.Threading.Tasks.Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        Log.Error($"Exceprion: {exception.Message}"); // Print exception
        await System.Threading.Tasks.Task.CompletedTask; // Finish async method
    }

    // Async method can be implemented wiht other method  at the same time
    // Task means, that program can start this method and do other actions during working of the method
    private async System.Threading.Tasks.Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token) // Is used after each update
    {
        OnMessage?.Invoke(client, update); // Get message and start working with it
        await System.Threading.Tasks.Task.CompletedTask; // Finish async method
    }
}
