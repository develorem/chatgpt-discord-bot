using Develorem.Discord.ChatGptBot;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

public class ChatGptBot
{
    private readonly DiscordSocketClient _client;
    private readonly IChatGptApi _chatGptApi;
    private readonly ILogger<ChatGptBot> _logger;
    private readonly IConfiguration _configuration;

    public ChatGptBot(DiscordSocketClient client, IChatGptApi chatGptApi, ILogger<ChatGptBot> logger, IConfiguration configuration)
    {
        _client = client;
        _chatGptApi = chatGptApi;
        _logger = logger;
        _configuration = configuration;

        _client.Log += LogAsync;
        _client.MessageReceived += MessageReceivedAsync;
    }

    public async Task MainAsync()
    {
        string token = _configuration["DiscordBotToken"]; // Retrieve the Discord bot token from configuration

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();

        // Keep the bot running until you stop it manually
        await Task.Delay(-1);
    }

    private Task LogAsync(LogMessage log)
    {
        _logger.LogInformation(log.ToString());
        return Task.CompletedTask;
    }

    private async Task MessageReceivedAsync(SocketMessage message)
    {
        if (message.Author.Id == _client.CurrentUser.Id) return;

        if (message.Content.StartsWith("!ask"))
        {
            string userMessage = message.Content.Substring(5).Trim();
            string response = await _chatGptApi.GetAiResponse(userMessage);

            await message.Channel.SendMessageAsync(response);
        }
    }
}