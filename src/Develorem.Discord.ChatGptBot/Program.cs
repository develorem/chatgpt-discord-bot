using Develorem.Discord.ChatGptBot;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = new ServiceCollection()
    .AddLogging()
    .AddSingleton<DiscordSocketClient>()
    .AddTransient<IChatGptApi, ChatGptApi>(provider =>
    {
        var configuration = provider.GetRequiredService<IConfiguration>();
        var openAiApiKey = configuration["OpenAiApiKey"]; // Retrieve the OpenAI API key from configuration
        return new ChatGptApi(openAiApiKey);
    })
    .AddTransient<ChatGptBot>()
    .BuildServiceProvider();

var bot = serviceProvider.GetRequiredService<ChatGptBot>();
bot.MainAsync().GetAwaiter().GetResult();