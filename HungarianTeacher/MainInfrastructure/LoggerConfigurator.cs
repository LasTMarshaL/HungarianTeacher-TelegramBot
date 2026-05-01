using Serilog;
namespace HungarianTeacher.MainInfrastructure
{
    public static class LoggerConfigurator 
    {
        public static void Setup() 
        {
            Log.Logger = new LoggerConfiguration() 
                .MinimumLevel.Debug()
                .WriteTo.Console() 
                .WriteTo.File("logs/bot_log_.txt", 
                    rollingInterval: RollingInterval.Day, 
                    retainedFileCountLimit: 7) 
                .CreateLogger(); 
        }
    }
}
