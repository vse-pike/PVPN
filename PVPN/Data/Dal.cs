using System.Data;
using Microsoft.Data.Sqlite;
using PVPN.Data.Model;

namespace PVPN.Data;

public class Dal : IDal
{
    private readonly IDbConnection _connection;

    public Dal(IDbConnection connection)
    {
        _connection = connection;
    }

    public void AddUser(User user)
    {
        using var command = _connection.CreateCommand();
        command.CommandText = @"
                INSERT INTO 'Users' (TelegramId, Name, CreatedAt)
                VALUES (@TelegramId, @Name, @CreatedAt)
                ";

        command.Parameters.Add(new SqliteParameter("@TelegramId", user.TelegramId));
        command.Parameters.Add(new SqliteParameter("@Name", user.Name));
        command.Parameters.Add(new SqliteParameter("@CreatedAt", user.CreatedDate));

        command.ExecuteNonQuery();
    }

    public User? GetUser(long telegramId)
    {
        using var command = _connection.CreateCommand();

        command.CommandText = @"
            SELECT * FROM 'Users'
            WHERE TelegramId = @TelegramId
        ";

        command.Parameters.Add(new SqliteParameter("@TelegramId", telegramId));

        using var reader = command.ExecuteReader();

        if (reader.Read())
        {
            var user = new User
            {
                TelegramId = reader.GetInt64(0),
                Name = reader.GetString(1),
                CreatedDate = reader.GetDateTime(2)
            };
            return user;
        }

        return null;
    }

    private string? GetAccessId(long telegramId)
    {
        string? accessId = null;

        using var command = _connection.CreateCommand();
        command.CommandText = @"
                SELECT AccessId FROM 'VpnAccess'
                WHERE TelegramId = @TelegramId;       
                ";

        command.Parameters.Add(new SqliteParameter("@TelegramId", telegramId));

        using var reader = command.ExecuteReader();

        while (reader.Read())
        {
            accessId = reader.GetString(0);
        }

        return accessId;
    }

    public void AddAccessAndTransaction(long telegramId, int port, string password, long amount)
    {
        using var transaction = _connection.BeginTransaction();

        try
        {
            var accessId = GetAccessId(telegramId);

            if (accessId == null)
            {
                using var command = _connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = @"
                        INSERT INTO 'VpnAccess'(AccessId, TelegramId, Port, Password, StartDate, EndDate, IsActive)
                        VALUES(@AccessId, @TelegramId, @Port, @Password, @StartDate, @EndDate, @IsActive);
                        ";

                accessId = Guid.NewGuid().ToString();

                command.Parameters.Add(new SqliteParameter("@AccessId", accessId));
                command.Parameters.Add(new SqliteParameter("@TelegramId", telegramId));
                command.Parameters.Add(new SqliteParameter("@Port", port));
                command.Parameters.Add(new SqliteParameter("@Password", password));
                command.Parameters.Add(new SqliteParameter("@StartDate", DateTime.Now));
                command.Parameters.Add(new SqliteParameter("@EndDate", DateTime.Now.AddMonths(1)));
                command.Parameters.Add(new SqliteParameter("@IsActive", 1));

                command.ExecuteNonQuery();
            }
            else
            {
                using var command = _connection.CreateCommand();
                command.Transaction = transaction;

                command.CommandText = @"
                        UPDATE 'VpnAccess'
                        SET Port = @Port, Password = @Password, StartDate = @StartDate, EndDate = @EndDate, IsActive = @IsActive
                        WHERE TelegramId = @TelegramId;
                        ";

                command.Parameters.Add(new SqliteParameter("@TelegramId", telegramId));
                command.Parameters.Add(new SqliteParameter("@Port", port));
                command.Parameters.Add(new SqliteParameter("@Password", password));
                command.Parameters.Add(new SqliteParameter("@StartDate", DateTime.Now));
                command.Parameters.Add(new SqliteParameter("@EndDate", DateTime.Now.AddMonths(1)));
                command.Parameters.Add(new SqliteParameter("@IsActive", 1));

                command.ExecuteNonQuery();
            }

            using (var command = _connection.CreateCommand())
            {
                command.Transaction = transaction;
                command.CommandText = @"
                        INSERT INTO 'Transactions'(TelegramId, Amount, TransactionDate, AccessId)
                        VALUES(@TelegramId, @Amount, @TransactionDate, @AccessId)
                        ";

                command.Parameters.Add(new SqliteParameter("@TelegramId", telegramId));
                command.Parameters.Add(new SqliteParameter("@Amount", amount));
                command.Parameters.Add(new SqliteParameter("@TransactionDate",
                    DateOnly.FromDateTime(DateTime.UtcNow)));
                command.Parameters.Add(new SqliteParameter("@AccessId", accessId));

                command.ExecuteNonQuery();
            }

            transaction.Commit();
        }
        catch (Exception e)
        {
            transaction.Rollback();
            Console.WriteLine(e);
        }
    }

    public void SetIsActive(string accessId, int isActive)
    {
        using var command = _connection.CreateCommand();

        command.CommandText = @"
                UPDATE 'VpnAccess'
                SET IsActive = @IsActive
                WHERE AccessId = @AccessId
                ";

        command.Parameters.Add(new SqliteParameter("@IsActive", isActive));
        command.Parameters.Add(new SqliteParameter("@AccessId", accessId));

        command.ExecuteNonQuery();
    }

    public List<int> GetAllAccessIdsWithEndDates()
    {
        using var command = _connection.CreateCommand();

        var currentDate = DateTime.Now;

        command.CommandText = @"
                SELECT AccessId FROM 'VpnAccess'
                WHERE EndDate < @EndDate
                ";

        command.Parameters.Add(new SqliteParameter("@EndDate", currentDate));

        var reader = command.ExecuteReader();

        var accessIdsList = new List<int>();

        while (reader.Read())
        {
            var accessId = reader.GetInt32(0);
            accessIdsList.Add(accessId);
        }

        return accessIdsList;
    }
}