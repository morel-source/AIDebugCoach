namespace AiDebugCoach.Models;

public sealed record GenerateRequest
{
    public required string Code { get; init; }
    public required string ErrorMessage { get; init; }
}