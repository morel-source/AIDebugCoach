using AiDebugCoach.Models;

namespace AiDebugCoach.Services;

public interface IAiService
{
    Task<GenerateContentResult> AnalyzeCode(GenerateRequest generateRequest,
        CancellationToken cancellationToken = default);
}