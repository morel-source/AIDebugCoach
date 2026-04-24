using AiDebugCoach.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDebugCoach.ViewComponents;

public class HistorySidebarViewComponent(BugReportService bugReportService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync(CancellationToken cancellationToken = default)
    {
        var history = await bugReportService.GetAllReportsAsync(cancellationToken);
        return View(history);
    }
}