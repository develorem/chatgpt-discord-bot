using Discord.WebSocket;
using Discord;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Develorem.Discord.ChatGptBot
{
    public class DiscordBotService : IHostedService
    {
        private readonly DiscordSocketClient _discordClient;
        private readonly IChatGptService _chatGptService;
        private readonly IConfiguration _configuration;
        private readonly ILogger<DiscordBotService> _logger;

        public DiscordBotService(
            DiscordSocketClient discordClient,
            IChatGptService chatGptService,
            IConfiguration configuration,
            ILogger<DiscordBotService> logger)
        {
            _discordClient = discordClient;
            _chatGptService = chatGptService;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task StartAsync(CancellationToken cancellationToken)
        {
            try
            {
                
                _discordClient.Log += LogMessage;
                _discordClient.MessageReceived += MessageReceivedAsync;

                string token = _configuration["DiscordToken"];                

                await _discordClient.LoginAsync(TokenType.Bot, token);
                await _discordClient.StartAsync();

                _logger.LogInformation("Bot started successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting the hosted service");
            }
        }

        public async Task StopAsync(CancellationToken cancellationToken)
        {
            await _discordClient.StopAsync();
            await _discordClient.LogoutAsync();

            _logger.LogInformation("Bot stopped successfully");
        }

        private Task LogMessage(LogMessage msg)
        {
            _logger.LogInformation("MSG received: " + msg.Message);
            return Task.CompletedTask;
        }

        private async Task MessageReceivedAsync(SocketMessage message)
        {
            if (message.Author.Id == _discordClient.CurrentUser.Id) return;

            if (message.Content.StartsWith("!ask"))
            {
                string userMessage = message.Content.Substring(5).Trim();
                string response = await _chatGptService.GetChatGptResponse(userMessage);

                await message.Channel.SendMessageAsync(response);
            }
        }
    }
}
