using Google.GenAI;
using Google.GenAI.Types;
using System.Text.Json.Nodes;

namespace WontonUpAPI.Services
{
    public class GeminiService : IAiService
    {
        private readonly string _apiKey;

        public GeminiService(IConfiguration config)
        {
            _apiKey = config["GEMINI_API_KEY"];
        }

        public async Task<string> SendPrompt(string systemPrompt, string userPrompt)
        {
            var client = new Client(apiKey: _apiKey);

            string outputSchema = @"
            {
              ""type"": ""object"",
              ""properties"": {
                ""items"": { 
                    ""type"": ""object"", 
                    ""properties"": {
                        ""name"": { ""type"": ""string"", ""name"": ""Name"" },
                        ""typeOfProduct"": { ""type"": ""string"", ""typeOfProduct"": ""typeOfProduct"" },
                        ""why"": { ""type"": ""string"", ""why"": ""Why"" }
                    },
                    ""required"": [""name"", ""typeOfProduct"", ""why""]
                }
              },
              ""required"": [""items""]
            }";

            GenerateContentConfig config = new()
            {
                ResponseJsonSchema = JsonNode.Parse(outputSchema),
                ResponseMimeType = "application/json",
                SystemInstruction = new Content
                {
                    Parts = [
                        new Part {
                            Text = systemPrompt,
                        }
                    ]
                }
            };

            var response = await client.Models.GenerateContentAsync(model: "gemini-3.5-flash", contents: userPrompt, config);

            Console.WriteLine(response.Candidates[0].Content.Parts[0].Text);

            return response.Candidates[0].Content.Parts[0].Text;
        }
    }
}
