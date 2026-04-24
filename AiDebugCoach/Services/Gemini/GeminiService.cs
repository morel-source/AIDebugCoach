using System.Text.Json;
using AiDebugCoach.Models;
using GenerativeAI;
using GenerativeAI.Types;
using Polly;
using Polly.Retry;

namespace AiDebugCoach.Services.Gemini;

public sealed class GeminiService(
    ILogger<GeminiService> logger,
    GenerativeModel model
) : IAiService
{
    private readonly string _contentSchema = GenerateContentResult.GenerateSchema();

    // Enhanced Retry Policy with Logging
    private readonly AsyncRetryPolicy _retryPolicy = Policy
        .Handle<Exception>(ex => ex.Message.Contains("503") || ex.Message.Contains("500"))
        .WaitAndRetryAsync(
            retryCount: 3,
            sleepDurationProvider: retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
            onRetry: (exception, timeSpan, retryCount, context) =>
            {
                logger.LogWarning("Retry {Count} failed: {Message}. Waiting {Delay}s before next attempt.",
                    retryCount, exception.Message, timeSpan.TotalSeconds);
            });


    public async Task<GenerateContentResult> AnalyzeCode(GenerateRequest generateRequest,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Starting AI Analysis for error: {ErrorMessage}", generateRequest.ErrorMessage);

        try
        {
            var requestBody = ConstructRequestBody(generateRequest);

            var response = await _retryPolicy.ExecuteAsync(async () =>
            {
                logger.LogDebug("Sending request to Gemini API...");
                return await model.GenerateContentAsync(requestBody, cancellationToken: cancellationToken)
                    .ConfigureAwait(false);
            });

            var result = ExtractResponseText(response);

            logger.LogInformation("Successfully received and parsed AI response.");
            return result;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Failed to analyze code after retries. Error: {ErrorMessage}", ex.Message);
            throw;
        }
    }

    private string ConstructRequestBody(GenerateRequest generateRequest) =>
        $"""
         You are an Senior C# Developer. Analyze the following:
         ERROR: {generateRequest.ErrorMessage}
         CODE: {generateRequest.Code}
         Return the response ONLY as a JSON schema:
         {_contentSchema}
         """;

    private GenerateContentResult ExtractResponseText(GenerateContentResponse response)
    {
        var rawText = response.Text;

        if (string.IsNullOrWhiteSpace(rawText))
        {
            logger.LogError("Received empty response from AI model.");
            throw new InvalidOperationException("AI returned an empty response.");
        }

        var cleanJson = rawText.Replace("```json", "").Replace("```", "").Trim();

        try
        {
            var result = JsonSerializer.Deserialize<GenerateContentResult?>(cleanJson, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            return result ?? throw new JsonException("Deserialization returned null.");
        }
        catch (JsonException ex)
        {
            logger.LogError(ex, message: "Failed to parse AI response. Raw output: {RawText}", cleanJson);
            throw;
        }
    }
}