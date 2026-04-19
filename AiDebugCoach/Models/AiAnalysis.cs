using System.Text.Json.Serialization;

namespace AiDebugCoach.Models;

public sealed class AiAnalysis
{
    [JsonPropertyName("cause")]
    public required string Cause { get; init; }

    [JsonPropertyName("fix")]
    public required string Fix { get; init; }
}