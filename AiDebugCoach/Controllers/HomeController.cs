using AiDebugCoach.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDebugCoach.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    AiService aiService,
    BugReportService bugReportService
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(CancellationToken cancellationToken = default)
    {
        var history = await bugReportService.GetAllReportsAsync(cancellationToken);
        return View(history);
    }

    [HttpPost]
    public async Task<IActionResult> Analyze(string code, string errorMessage, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(code) || string.IsNullOrWhiteSpace(errorMessage))
        {
            TempData["Error"] = "Please provide both code and an error message.";
            return RedirectToAction("Index");
        }

        try
        {
            var result = await aiService.AnalyzeCode(code, errorMessage, ct);
            await bugReportService.AddReportAsync(code, errorMessage, result, ct);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, message: "Error while analyzing code: {message}", ex.Message);
            TempData["Error"] = "The AI service failed to respond. Please check your API key or connection.";
        }

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await bugReportService.DeleteReportAsync(id);
        return RedirectToAction("Index");
    }
}