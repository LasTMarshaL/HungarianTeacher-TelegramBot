using HungarianTeacher.MainInfrastructure;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;

public class TelegramBotHost
{
    public Action<ITelegramBotClient, Update>? OnMessage;
    public TelegramBotClient telegramBotClient;

    public TelegramBotHost(string _token) 
    {
        telegramBotClient = new TelegramBotClient(_token); 
    }

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

    private async System.Threading.Tasks.Task ErrorHandler(ITelegramBotClient client, Exception exception, HandleErrorSource source, CancellationToken token)
    {
        Log.Error($"Exceprion: {exception.Message}"); 
        await System.Threading.Tasks.Task.CompletedTask;
    }

    private async System.Threading.Tasks.Task UpdateHandler(ITelegramBotClient client, Update update, CancellationToken token)
    {
        OnMessage?.Invoke(client, update); 
        await System.Threading.Tasks.Task.CompletedTask; 
    }
}
