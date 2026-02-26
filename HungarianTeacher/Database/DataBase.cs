using HungarianTeacher.Database;
using Microsoft.Data.Sqlite;


public class Database: IDatabase // This class is responsiable for working with database
{
    public async Task<string> GetDatabasePath() // Get path to the database
    { 
        // Environment gives access to special folders in the system
        // LocalApplicationData - folder for application data for current user
        var folder = Environment.SpecialFolder.LocalApplicationData;

        // Get path to the folder for application data for current user and add name of the database file to it
        var path = Environment.GetFolderPath(folder);

        // Join path to the folder and name of the database file to get full path to the database
        var databasePath = System.IO.Path.Join(path, "TelegramBotDatabase.db");

        return databasePath; // Return path to the database
    }

    public async Task CreateDatabaseTable() // Create database if it doesn't exist
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command

        // SQLite3 doesn't support boolean data type, so we use INTEGER with values 0 and 1 to represent false and true respectively
        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                ChatID TEXT PRIMARY KEY,
                IsWaitingForLanguageMessage INTEGER DEFAULT 0,
                IsWaitingForMinutesMessage INTEGER DEFAULT 0,
                Minutes INTEGER DEFAULT 30,
                TargetTime TEXT
            );
        "; // SQL-request to create table for users' Telegram chat IDs and their settings

        await command.ExecuteNonQueryAsync(); // Execute SQL-request without any feedback
    }
    public async Task AddNewChatID(string chatID) // Add new Telegram chat id 
    {
        string databasePath = await GetDatabasePath(); // Get path to the database 

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"
                    INSERT OR IGNORE INTO Users (ChatID)
                    VALUES ($chatID);
                    "; // SQL-request to add new user's Telegram chat ID

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request

        await command.ExecuteNonQueryAsync(); // Execute SQL-request without any feedback
    }

    public async Task<List<string>> GetAllChatIDs() // Get all users' Telegram chat IDs
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        List<string> chatIDs = new List<string>(); // Create list for users' Telegram chat IDs

        using var connection = new SqliteConnection($"Data Source={databasePath}");// Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"
                              SELECT ChatID FROM Users
                              ;"; // SQL-request to get all chat IDs

        using var reader = await command.ExecuteReaderAsync(); // Create SQL-request to the database to get data
        while (reader.Read())
        {
            chatIDs.Add(reader.GetString(0)); // Add chat ID to the list
        }

        return chatIDs; // Return list with all users' Telegram chat IDs
    }

    public async Task SetIsWaitingForLanguageMessage(string chatID, bool isWaitingForLanguageMessage) // Set value to check if Telegram bot is waitng for user's message to select language
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"
            UPDATE Users
            SET IsWaitingForLanguageMessage = $isWaitingForLanguageMessage
            WHERE ChatID = $chatID;
            "; // SQL-request to get if Telegram bot is waitng for user's message to select language 

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request
        command.Parameters.AddWithValue("$isWaitingForLanguageMessage", isWaitingForLanguageMessage); // Bind the value of the category to the parameter of the SQL-request

        await command.ExecuteNonQueryAsync(); // Execute SQL-request without any feedback
    }

    public async Task<bool> GetIsWaitingForLanguageMessage(string chatID) // Set value to check if Telegram bot is waitng for user's message to set time between messages
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        bool isWaitingForLanguageMessage = false; // Craete variable for value to check if Telegram bot is waitng for user's message to select language

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"SELECT IsWaitingForLanguageMessage From USERS
                              WHERE ChatID = $chatID
                              "; // SQL-request to get if Telegram bot is waitng for user's message to select language

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request

        using var reader = await command.ExecuteReaderAsync(); // Create SQL-request to the database to get data
        if (await reader.ReadAsync())
        {
            if (!reader.IsDBNull(0)) // Check if the value is null
            {
                isWaitingForLanguageMessage = reader.GetBoolean(0); // Take value from captured string
            }
        }

        return isWaitingForLanguageMessage; // Return received value
    }

    public async Task SetIsWaitingForMinutesMessage(string chatID, bool isWaitingForMinutesMessage) // Set value to check if Telegram bot is waitng for user's message to set time between messages
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"
            UPDATE Users
            SET IsWaitingForMinutesMessage = $isWaitingForMinutesMessage
            WHERE ChatID = $chatID;
            "; // SQL-request to get if Telegram bot is waitng for user's message to select language 

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request
        command.Parameters.AddWithValue("$isWaitingForMinutesMessage", isWaitingForMinutesMessage); // Bind the value of the category to the parameter of the SQL-request

        await command.ExecuteNonQueryAsync(); // Execute SQL-request without any feedback
    }


    public async Task<bool> GetIsWaitingForMinutesMessage(string chatID) // Set value to check if Telegram bot is waitng for user's message to set time between messages
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        bool isWaitingForMinutesMessage = false; // Craete variable for value to check if Telegram bot is waitng for user's message to select language

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"SELECT IsWaitingForMinutesMessage From USERS
                              WHERE ChatID = $chatID
                              "; // SQL-request to get if Telegram bot is waitng for user's message to select language

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request

        using var reader = await command.ExecuteReaderAsync(); // Create SQL-request to the database to get data
        if (await reader.ReadAsync())
        {
            if (!reader.IsDBNull(0)) // Check if the value is null
            {
                isWaitingForMinutesMessage = reader.GetBoolean(0); // Take value from captured string
            }
        }

        return isWaitingForMinutesMessage; // Return received value
    }

    public async Task SetTimeBetweenMessageAndTargetTime(string chatID, int minutes, string targetTime) // Set value to set time between sending messages and target time to send message
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"
        UPDATE Users
        SET Minutes = $minutes,
            TargetTime = $targetTime
        WHERE ChatID = $chatID
        "; // SQL-request to get if Telegram bot is waitng for user's message to select language 

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request
        command.Parameters.AddWithValue("$minutes", minutes); // Bind the value of the category to the parameter of the SQL-request
        command.Parameters.AddWithValue("$targetTime", targetTime); // Bind the value of the category to the parameter of the SQL-request

        await command.ExecuteNonQueryAsync(); // Execute SQL-request without any feedback
    }

    public async Task<int> GetTimeBetweenMessages(string chatID) // Get value to set time between sending messages
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        int minutes = 30; // Set based value of varibable

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"SELECT Minutes FROM Users
            WHERE ChatID = $chatID"; // SQL-request to get if Telegram bot is waitng for user's message to select language

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request

        using var reader = await command.ExecuteReaderAsync(); // Create SQL-request to the database to get data
        if (await reader.ReadAsync())
        {
            minutes = reader.GetInt32(0); // Take value from captured string
        }

        return minutes; // Return received value
    }

    public async Task SetTargetTime(string chatID, string targetTime) // Set target time to send message
    {
        string databasePath = await GetDatabasePath(); // Get path to the database

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"UPDATE users
            SET TargetTime = $targetTime
            WHERE ChatID = $chatID";  // SQL-request to get if Telegram bot is waitng for user's message to select language 

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request
        command.Parameters.AddWithValue("$targetTime", targetTime); // Bind the value of the category to the parameter of the SQL-request

        await command.ExecuteNonQueryAsync(); // Execute SQL-request without any feedback
    }

    public async Task<string> GetTargetTime(string chatID) // Get target time to send message
    {
        string targetTime = ""; // Create variable for target time

        string databasePath = await GetDatabasePath(); // Get path to the database

        using var connection = new SqliteConnection($"Data Source={databasePath}"); // Connect to the database
        await connection.OpenAsync(); // Open connection

        var command = connection.CreateCommand(); // Create command
        command.CommandText = @"SELECT TargetTime FROM Users
            WHERE ChatID = $chatID"; // SQL-request to get if Telegram bot is waitng for user's message to select language

        command.Parameters.AddWithValue("$chatID", chatID); // Bind the value of the category to the parameter of the SQL-request

        using var reader = await command.ExecuteReaderAsync(); // Create SQL-request to the database to get data
        if (await reader.ReadAsync())
        {
            targetTime = reader.GetString(0); // Take value from captured string
                                        
        }

        return targetTime; // Return time to send next message
    }
}
