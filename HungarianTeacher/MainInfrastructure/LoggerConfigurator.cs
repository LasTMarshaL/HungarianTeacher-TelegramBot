using Serilog;

namespace HungarianTeacher.MainInfrastructure
{
    public static class LoggerConfigurator // This class is responisble for centralized configuration of the logging system.
    {
        public static void Setup() // Initialize a global logger with console output and cyclical writing to the file
        {
            Log.Logger = new LoggerConfiguration() // Hand over the logger configuration 
                .MinimumLevel.Debug() // Set the minimum log level to Debug (all log messages with a level of Debug or higher will be logged)
                .WriteTo.Console() // Write log messages to the console
                .WriteTo.File("logs/bot_log_.txt", // Write log messages to a file with the specified path and name pattern
                    rollingInterval: RollingInterval.Day, // Create a new log file every day (to not overload the storage)
                    retainedFileCountLimit: 7) // Keep only the last 7 log files (to not overload the storage)
                .CreateLogger(); // Create the logger instance with the specified configuration
        }
    }
}
