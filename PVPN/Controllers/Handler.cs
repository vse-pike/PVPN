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

        // set the process start info
        process.StartInfo.FileName = "add_vpn_access.sh"; // specify the command to run
        process.StartInfo.Arguments = $"{telegramId} {port} {password}";

        // set additional process start info as necessary
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        // start the process
        process.Start();

        // read the output from the command
        string output = await process.StandardOutput.ReadToEndAsync();

        // wait for the process to exit
        await process.WaitForExitAsync();

        // print the output
        Console.WriteLine(output);
    }

    public async Task RevokeVpnAccessAsync(long telegramId)
    {
        Process process = new Process();

        // set the process start info
        process.StartInfo.FileName = "revoke_vpn_access.sh"; // specify the command to run
        process.StartInfo.Arguments = $"{telegramId}";

        // set additional process start info as necessary
        process.StartInfo.UseShellExecute = false;
        process.StartInfo.RedirectStandardOutput = true;

        // start the process
        process.Start();

        // read the output from the command
        string output = await process.StandardOutput.ReadToEndAsync();

        // wait for the process to exit
        await process.WaitForExitAsync();

        // print the output
        Console.WriteLine(output);
    }
}