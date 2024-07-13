using PVPN.Data.Model;

namespace PVPN.Controllers;

public interface IHandler
{
    public Task CreateUser(User user);
    public Task AddAccessAndTransaction(long telegramId, int port, string password, long amount);
    public Task<bool> CheckUserShouldExist(long telegramId);
    public Task AddVpnAccess(long telegramId, int port, string password);
}