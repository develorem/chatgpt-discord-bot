using Develorem.Discord.ChatGptBot;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System;
using System.Net.Http.Headers;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

public interface IChatGptService
{
    Task<string> GetChatGptResponse(string input);
}

public class ChatGptService : IChatGptService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;    
    private readonly ILogger<ChatGptService> _logger;

    public ChatGptService(
        HttpClient httpClient,
        IConfiguration configuration,
        ILogger<ChatGptService> logger)
    {
        _httpClient = httpClient;
        _configuration = configuration;        
        _logger = logger;
    }

    public async Task<string> GetChatGptResponse(string input)
    {
        try
        {
            var body = "{ \"model\": \"gpt-3.5-turbo\",  \"messages\": [{\"role\": \"user\", \"content\": \"$input$\"}], \"temperature\": 0.7 }";
            body = body.Replace("$input$", input);

            var content = new StringContent(body, Encoding.UTF8, "application/json");

            var key = _configuration["OpenAIApiKey"];

            _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", key);

            var response = await _httpClient.PostAsync("https://api.openai.com/v1/chat/completions", content);

            response.EnsureSuccessStatusCode();

            var responseBody = await response.Content.ReadAsStringAsync();

            // Parse the response from the GPT-3 API and return the generated text.
            var result = JsonConvert.DeserializeObject<dynamic>(responseBody);
            var resultBody = result.choices[0].message.content;

            return resultBody;
        }
        catch (HttpRequestException ex)
        {
            _logger.LogError("Error calling GPT-3 API: {ErrorMessage}", ex.Message);
            throw;
        }
    }
}

