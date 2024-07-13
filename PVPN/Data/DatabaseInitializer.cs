using System.Data;

namespace PVPN.Data;

public class DatabaseInitializer
{
    private readonly IDbConnection _connection;

    public DatabaseInitializer(IDbConnection connection)
    {
        _connection = connection;
    }

    public void Initialize()
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
                CREATE TABLE IF NOT EXISTS Users (
                    TelegramId INTEGER PRIMARY KEY NOT NULL UNIQUE,
                    Name TEXT NOT NULL,
                    CreatedAt TEXT NOT NULL
                );

                CREATE TABLE IF NOT EXISTS VpnAccess (
                    AccessId TEXT PRIMARY KEY NOT NULL,
                    TelegramId INTEGER NOT NULL UNIQUE,
                    Port INTEGER NOT NULL,
                    Password TEXT NOT NULL,
                    StartDate TEXT NOT NULL,
                    EndDate TEXT NOT NULL,
                    IsActive INTEGER NOT NULL,
                    FOREIGN KEY (TelegramId) REFERENCES Users(TelegramId)
                );

                CREATE TABLE IF NOT EXISTS Transactions (
                    TransactionId INTEGER PRIMARY KEY AUTOINCREMENT NOT NULL,
                    TelegramId INTEGER NOT NULL,
                    Amount INTEGER NOT NULL,
                    TransactionDate TEXT NOT NULL,
                    AccessId INTEGER NOT NULL,
                    FOREIGN KEY (AccessId) REFERENCES VpnAccess(AccessId),
                    FOREIGN KEY (TelegramId) REFERENCES Users(TelegramId)
                );
            ";

        command.ExecuteNonQuery();
    }
}