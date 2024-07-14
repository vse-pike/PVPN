using System.Data;
using PVPN.Data;
using Microsoft.Data.Sqlite;
using PVPN.BotClient;
using PVPN.BotClient.Commands;
using PVPN.Controllers;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<IDbConnection>(_ =>
{
    var connectionString = "Data Source=database.db";
    var connection = new SqliteConnection(connectionString);
    connection.Open();
    return connection;
});
builder.Services.AddScoped<DatabaseInitializer>();
builder.Services.AddScoped<IDal, Dal>();
builder.Services.AddHttpClient("telegram_bot")
    .AddTypedClient<ITelegramBotClient>(httpClient => 
{
    TelegramBotClientOptions options = new TelegramBotClientOptions("6298565882:AAEbeyIie_F1xL-9QKOu0G03TwjjS95tUgk");

    return new TelegramBotClient(options, httpClient);
});

builder.Services.AddScoped<UpdateHandler>();
builder.Services.AddScoped<ReceiverService>();
builder.Services.AddHostedService<PollingService>();
builder.Services.AddScoped<IHandler, Handler>();
builder.Services.AddScoped<IUpdateHandler, UpdateHandler>();
builder.Services.AddScoped<CommandContainer>();
builder.Services.AddScoped<StartCommand>();
builder.Services.AddScoped<PayCommand>();
builder.Services.AddScoped<AddAccessCommand>();

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var dbInit = scope.ServiceProvider.GetService<DatabaseInitializer>();

    dbInit?.Initialize();
}

app.Run();