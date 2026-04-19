using System.Text;
using System.Text.Json;

namespace AiDebugCoach.Services;

public sealed class AiService(HttpClient httpClient, IConfiguration config)
{
    public async Task<string> AnalyzeCode(string code, string errorMessage,
        CancellationToken cancellationToken = default)
    {
        var apiKey = config["GeminiApiKey"];
        var modelName = config["GeminiSettings:ModelName"];
        var url = $"https://generativelanguage.googleapis.com/v1/{modelName}:generateContent?key={apiKey}";

        var messageText = $@"You are a Senior C# Developer. Analyze the following:
                        ERROR: {errorMessage}
                        CODE: {code}
                        Return the response ONLY as a JSON object:
                        {{ ""cause"": ""explanation"", ""fix"": ""code examples"" }}";

        var body = new { contents = new[] { new { parts = new[] { new { text = messageText } } } } };
        var json = JsonSerializer.Serialize(body);
        var content = new StringContent(json, Encoding.UTF8, "application/json");

        var response = await httpClient.PostAsync(url, content, cancellationToken);

        if (!response.IsSuccessStatusCode)
        {
            var errorContent = await response.Content.ReadAsStringAsync(cancellationToken);
            throw new Exception($"API Error: {response.StatusCode} - {errorContent}");
        }

        var responseString = await response.Content.ReadAsStringAsync(cancellationToken);

        using var doc = JsonDocument.Parse(responseString);
        var root = doc.RootElement;

        string? aiText = null;

        try
        {
            var candidates = root.GetProperty("candidates");
            var firstCandidate = candidates.EnumerateArray().FirstOrDefault();

            if (firstCandidate.ValueKind != JsonValueKind.Undefined)
            {
                aiText = firstCandidate.GetProperty("content")
                    .GetProperty("parts")[0]
                    .GetProperty("text")
                    .GetString();
            }
        }
        catch (KeyNotFoundException)
        {
            throw new Exception("The API returned an unexpected response format.");
        }

        aiText = aiText?.Replace("```json", "").Replace("```", "").Trim();

        return aiText ?? "{}";
    }
}