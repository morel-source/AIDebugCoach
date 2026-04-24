using AiDebugCoach.Models;
using AiDebugCoach.Models.ViewModels;
using AiDebugCoach.Services;
using Microsoft.AspNetCore.Mvc;

namespace AiDebugCoach.Controllers;

public class HomeController(
    ILogger<HomeController> logger,
    IAiService aiService,
    BugReportService bugReportService
) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(int? id, CancellationToken cancellationToken = default)
    {
        var reports = await bugReportService.GetAllReportsAsync(cancellationToken);

        var viewModelList = reports.Select(r =>
            new BugReportViewModel(
                Id: r.Id,
                Code: r.Code,
                ErrorMessage: r.ErrorMessage,
                FormattedDate: r.CreatedAt.ToString("MMM dd, HH:mm"),
                Analysis: r.AiResponse
            )).ToList();

        if (!id.HasValue) return View(viewModelList);

        var selected = await bugReportService.GetReportByIdAsync(id.Value, cancellationToken);
        if (selected != null)
        {
            ViewBag.SelectedReport = new BugReportViewModel(
                Id: selected.Id,
                Code: selected.Code,
                ErrorMessage: selected.ErrorMessage,
                FormattedDate: selected.CreatedAt.ToString("MMM dd, HH:mm"),
                Analysis: selected.AiResponse
            );
        }

        return View(viewModelList);
    }

    [HttpPost]
    public async Task<IActionResult> Analyze([FromBody] GenerateRequest generateRequest,
        CancellationToken cancellationToken = default)
    {
        try
        {
            var analyzeResult = await aiService.AnalyzeCode(generateRequest, cancellationToken);
            await bugReportService.AddReportAsync(generateRequest, analyzeResult, cancellationToken);

            return Json(new { success = true, result = analyzeResult });
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "API Error: {Message}", ex.Message);

            if (ex.Message.Contains("429"))
                return StatusCode(StatusCodes.Status429TooManyRequests, "Daily limit reached.");

            if (ex.Message.Contains("503") || ex.Message.Contains("UNAVAILABLE"))
                return StatusCode(StatusCodes.Status503ServiceUnavailable, "AI is busy. Try again soon.");

            return BadRequest("Something went wrong.");
        }
    }

    public async Task<IActionResult> Details(int id, CancellationToken cancellationToken = default)
    {
        var report = await bugReportService.GetReportByIdAsync(id, cancellationToken);
        ViewBag.SelectedReport = report;

        return View("Index");
    }

    [HttpPost]
    public async Task<IActionResult> Delete(int id)
    {
        await bugReportService.DeleteReportAsync(id);
        return RedirectToAction("Index");
    }
}