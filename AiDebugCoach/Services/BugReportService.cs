using AiDebugCoach.Data;
using AiDebugCoach.Models;
using Microsoft.EntityFrameworkCore;

namespace AiDebugCoach.Services;

public sealed class BugReportService(AppDbContext db)
{
    public async Task<List<BugReport>> GetAllReportsAsync(CancellationToken cancellationToken = default)
    {
        return await db.BugReports
            .AsNoTracking()
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync(cancellationToken: cancellationToken);
    }

    public async Task AddReportAsync(string code, string errorMessage, string result,
        CancellationToken cancellationToken = default)
    {
        db.BugReports.Add(new BugReport
        {
            Code = code,
            AiResponse = result,
            CreatedAt = DateTime.UtcNow,
            ErrorMessage = errorMessage
        });

        await db.SaveChangesAsync(cancellationToken);
    }

    public async Task DeleteReportAsync(int id, CancellationToken cancellationToken = default)
    {
        var report = await db.BugReports.FindAsync([id], cancellationToken);
        if (report != null)
        {
            db.BugReports.Remove(report);
            await db.SaveChangesAsync(cancellationToken);
        }
    }
}