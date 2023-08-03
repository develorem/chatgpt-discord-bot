using Develorem.Discord.ChatGptBot;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static void Main(string[] args)
    {
        CreateHostBuilder(args).Build().Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureAppConfiguration((hostContext, config) =>
            {
                // For local development, put your keys and tokens into user secrets rather than appsettings where they would be committed to source control
                config.AddUserSecrets<Program>();                
            })
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<DiscordBotService>();
                services.AddHttpClient();
                services.Configure<OpenAIOptions>(hostContext.Configuration.GetSection("OpenAI"));
                services.AddSingleton<IChatGptService, ChatGptService>();
                services.AddSingleton<DiscordBotService>();

                var config = new DiscordSocketConfig() { GatewayIntents = Discord.GatewayIntents.All };
                services.AddSingleton<DiscordSocketClient>(p => new DiscordSocketClient(config));
                services.AddLogging();
            });
}