using PVPN.Data.Model;

namespace PVPN.Data;

public interface IDal
{
    public void AddUser(User user);
    public void AddAccessAndTransaction(long telegramId, int port, string password, long amount);
    public User? GetUser(long telegramId);
}