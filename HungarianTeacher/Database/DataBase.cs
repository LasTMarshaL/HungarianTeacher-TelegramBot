using HungarianTeacher.Database;
using Microsoft.Data.Sqlite;


public class Database: IDatabase 
{
    public async Task<string> GetDatabasePath() 
    { 
        var folder = Environment.SpecialFolder.LocalApplicationData;

        var path = Environment.GetFolderPath(folder);

        var databasePath = System.IO.Path.Join(path, "TelegramBotDatabase.db");

        return databasePath;
    }

    public async Task CreateDatabaseTable() 
    {
        string databasePath = await GetDatabasePath(); 

        using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand(); 

        command.CommandText = @"
            CREATE TABLE IF NOT EXISTS Users (
                ChatID TEXT PRIMARY KEY,
                IsWaitingForLanguageMessage INTEGER DEFAULT 0,
                IsWaitingForMinutesMessage INTEGER DEFAULT 0,
                Minutes INTEGER DEFAULT 30,
                TargetTime TEXT
            );
        ";

        await command.ExecuteNonQueryAsync();
    }

    public async Task AddNewChatID(string chatID) 
    {
        string databasePath = await GetDatabasePath();

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync();

        var command = connection.CreateCommand(); 
        command.CommandText = @"
                    INSERT OR IGNORE INTO Users (ChatID)
                    VALUES ($chatID);
                    "; 

        command.Parameters.AddWithValue("$chatID", chatID);

        await command.ExecuteNonQueryAsync(); 
    }

    public async Task<List<string>> GetAllChatIDs() 
    {
        string databasePath = await GetDatabasePath();

        List<string> chatIDs = new List<string>();

        using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
                              SELECT ChatID FROM Users
                              ;"; 

        using var reader = await command.ExecuteReaderAsync(); 
        while (reader.Read())
        {
            chatIDs.Add(reader.GetString(0));
        }

        return chatIDs;
    }
    public async Task SetIsWaitingForLanguageMessage(string chatID, bool isWaitingForLanguageMessage) 
    {
        string databasePath = await GetDatabasePath();

        using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand(); 
        command.CommandText = @"
            UPDATE Users
            SET IsWaitingForLanguageMessage = $isWaitingForLanguageMessage
            WHERE ChatID = $chatID;
            "; 

        command.Parameters.AddWithValue("$chatID", chatID);
        command.Parameters.AddWithValue("$isWaitingForLanguageMessage", isWaitingForLanguageMessage);

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> GetIsWaitingForLanguageMessage(string chatID) 
    {
        string databasePath = await GetDatabasePath(); 

        bool isWaitingForLanguageMessage = false; 

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT IsWaitingForLanguageMessage From USERS
                              WHERE ChatID = $chatID
                              "; 

        command.Parameters.AddWithValue("$chatID", chatID);

        using var reader = await command.ExecuteReaderAsync(); 
        if (await reader.ReadAsync())
        {
            if (!reader.IsDBNull(0)) 
            {
                isWaitingForLanguageMessage = reader.GetBoolean(0);
            }
        }

        return isWaitingForLanguageMessage;
    }

    public async Task SetIsWaitingForMinutesMessage(string chatID, bool isWaitingForMinutesMessage)
    {
        string databasePath = await GetDatabasePath(); 

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync(); 

        var command = connection.CreateCommand(); 
        command.CommandText = @"
            UPDATE Users
            SET IsWaitingForMinutesMessage = $isWaitingForMinutesMessage
            WHERE ChatID = $chatID;
            "; 

        command.Parameters.AddWithValue("$chatID", chatID);
        command.Parameters.AddWithValue("$isWaitingForMinutesMessage", isWaitingForMinutesMessage); 

        await command.ExecuteNonQueryAsync();
    }

    public async Task<bool> GetIsWaitingForMinutesMessage(string chatID) 
    {
        string databasePath = await GetDatabasePath();

        bool isWaitingForMinutesMessage = false;

        using var connection = new SqliteConnection($"Data Source={databasePath}");
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT IsWaitingForMinutesMessage From USERS
                              WHERE ChatID = $chatID
                              "; 

        command.Parameters.AddWithValue("$chatID", chatID); 

        using var reader = await command.ExecuteReaderAsync(); 
        if (await reader.ReadAsync())
        {
            if (!reader.IsDBNull(0))
            {
                isWaitingForMinutesMessage = reader.GetBoolean(0); 
            }
        }

        return isWaitingForMinutesMessage;
    }

    public async Task SetTimeBetweenMessageAndTargetTime(string chatID, int minutes, string targetTime) 
    {
        string databasePath = await GetDatabasePath();

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync();

        var command = connection.CreateCommand();
        command.CommandText = @"
        UPDATE Users
        SET Minutes = $minutes,
            TargetTime = $targetTime
        WHERE ChatID = $chatID
        "; 

        command.Parameters.AddWithValue("$chatID", chatID); 
        command.Parameters.AddWithValue("$minutes", minutes); 
        command.Parameters.AddWithValue("$targetTime", targetTime);

        await command.ExecuteNonQueryAsync();
    }


    public async Task<int> GetTimeBetweenMessages(string chatID)
    {
        string databasePath = await GetDatabasePath(); 

        int minutes = 30; 

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync(); 

        var command = connection.CreateCommand();
        command.CommandText = @"SELECT Minutes FROM Users
            WHERE ChatID = $chatID"; 

        command.Parameters.AddWithValue("$chatID", chatID); 

        using var reader = await command.ExecuteReaderAsync(); 
        if (await reader.ReadAsync())
        {
            minutes = reader.GetInt32(0);
        }

        return minutes;
    }


    public async Task SetTargetTime(string chatID, string targetTime) 
    {
        string databasePath = await GetDatabasePath(); 

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync();

        var command = connection.CreateCommand(); 
        command.CommandText = @"UPDATE users
            SET TargetTime = $targetTime
            WHERE ChatID = $chatID";  

        command.Parameters.AddWithValue("$chatID", chatID); 
        command.Parameters.AddWithValue("$targetTime", targetTime); 

        await command.ExecuteNonQueryAsync(); 
    }


    public async Task<string> GetTargetTime(string chatID) 
    {
        string targetTime = ""; 

        string databasePath = await GetDatabasePath(); 

        using var connection = new SqliteConnection($"Data Source={databasePath}"); 
        await connection.OpenAsync(); 

        var command = connection.CreateCommand(); 
        command.CommandText = @"SELECT TargetTime FROM Users
            WHERE ChatID = $chatID"; 

        command.Parameters.AddWithValue("$chatID", chatID); 

        using var reader = await command.ExecuteReaderAsync(); 
        if (await reader.ReadAsync())
        {
            targetTime = reader.GetString(0); 
                                        
        }

        return targetTime;
    }
}
