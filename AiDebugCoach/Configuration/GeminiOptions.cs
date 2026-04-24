namespace AiDebugCoach.Configuration;

public record GeminiOptions
{
    public const string SectionName = "GeminiSettings";
    public string ApiKey { get; init; } = string.Empty;
    public string ModelName { get; init; } = "gemini-pro";
}