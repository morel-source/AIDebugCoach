using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json;

namespace AiDebugCoach.Models;

public sealed record BugReport
{
    public int Id { get; init; }
    public required string Code { get; init; }
    public required string ErrorMessage { get; init; }
    public string AiResponseJson { get; init; }
    public DateTime CreatedAt { get; init; }


    [NotMapped]
    public GenerateContentResult? AiResponse =>
        string.IsNullOrEmpty(AiResponseJson) ? null : JsonSerializer.Deserialize<GenerateContentResult>(AiResponseJson);
}