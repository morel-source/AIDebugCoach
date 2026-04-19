using System.Text.Json;

namespace AiDebugCoach.Models;

public sealed class BugReport
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string ErrorMessage { get; init; }
    public required string AiResponse { get; init; }
    public DateTime CreatedAt { get; init; }

    public AiAnalysis? GetAnalysis()
    {
        try
        {
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            return JsonSerializer.Deserialize<AiAnalysis>(AiResponse, options);
        }
        catch
        {
            return null;
        }
    }
}