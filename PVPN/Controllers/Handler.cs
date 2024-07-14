using System.Diagnostics;
using PVPN.Data;
using PVPN.Data.Model;

namespace PVPN.Controllers;

public class Handler : IHandler
{
    private readonly IDal _dal;

    public Handler(IDal dal)
    {
        _dal = dal;
    }

    public Task CreateUser(User user)
    {
        Task.Run(() => { _dal.AddUser(user); });
        return Task.CompletedTask;
    }

    public Task AddAccessAndTransaction(long telegramId, int port, string password, long amount)
    {
        Task.Run(() => { _dal.AddAccessAndTransaction(telegramId, port, password, amount); });
        return Task.CompletedTask;
    }

    public Task<bool> CheckUserShouldExist(long telegramId)
    {
        var result = Task.Run(() =>
            _dal.GetUser(telegramId));
        return Task.FromResult(result.Result != null);
    }
    
    public async Task AddVpnAccess(long telegramId, int port, string password)
    {
        Process process = new Process();

        process.StartInfo.FileName = "add_shadowsocks_user.sh";
        process.StartInfo.Arguments = $"{telegramId} {password} {port}";
        Console.WriteLine($"{telegramId}:{password}:{port}");

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        Console.WriteLine(output);
    }

    public async Task RevokeVpnAccessAsync(long telegramId)
    {
        Process process = new Process();

        process.StartInfo.FileName = "revoke_shadowsocks_user.sh";
        process.StartInfo.Arguments = $"{telegramId}";

        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        process.Start();

        string output = await process.StandardOutput.ReadToEndAsync();

        await process.WaitForExitAsync();

        Console.WriteLine(output);
    }
}