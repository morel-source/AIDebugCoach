using NJsonSchema;

namespace AiDebugCoach.Models;

public sealed record GenerateContentResult
{
    public required string Cause { get; init; }
    public required string Explanation { get; init; }
    public required List<string> FixSteps { get; init; }
    public required string CodeExamples { get; init; }

    public static string GenerateSchema()
    {
        var schema = JsonSchema.FromType<GenerateContentResult>();
        return schema.ToJson();
    }
}