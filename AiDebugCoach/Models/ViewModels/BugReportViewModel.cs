namespace AiDebugCoach.Models.ViewModels;

public record BugReportViewModel(
    int Id,
    string Code,
    string ErrorMessage,
    string FormattedDate,
    GenerateContentResult? Analysis
);