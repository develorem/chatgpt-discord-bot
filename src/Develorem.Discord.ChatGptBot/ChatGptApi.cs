using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;

namespace Develorem.Discord.ChatGptBot
{
    public interface IChatGptApi
    {
        Task<string> GetAiResponse(string message);
    }

    public class ChatGptApi : IChatGptApi
    {
        private readonly HttpClient _httpClient;
        private readonly string _openAiApiKey;

        public ChatGptApi(string openAiApiKey)
        {
            _httpClient = new HttpClient();
            _openAiApiKey = openAiApiKey;
        }

        public async Task<string> GetAiResponse(string message)
        {
            try
            {
                var parameters = new Dictionary<string, string>
            {
                { "engine", "text-davinci-002" }, // Replace with the appropriate engine for your use case
                { "prompt", message },
                { "temperature", "0.7" },
                { "max_tokens", "150" }
            };

                _httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _openAiApiKey);

                var content = new FormUrlEncodedContent(parameters);
                var response = await _httpClient.PostAsync("https://api.openai.com/v1/engines/text-davinci-002/completions", content);
                var responseContent = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    // Handle rate limiting or API errors
                    return "Oops! An error occurred while processing your request.";
                }

                return responseContent;
            }
            catch (Exception ex)
            {
                // Handle any unexpected errors
                return "Oops! Something went wrong. Please try again later.";
            }
        }
    }
}
