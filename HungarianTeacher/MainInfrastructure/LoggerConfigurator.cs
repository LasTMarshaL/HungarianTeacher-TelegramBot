using Serilog;
namespace HungarianTeacher.MainInfrastructure
{
    public static class LoggerConfigurator // This class is responisble for centralized configuration of the logging system.
    {
        /// <summary>
        /// Configures the application's logging system to use a debug-level logger that writes output to both the console and a daily rolling log file.
        /// </summary>
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
