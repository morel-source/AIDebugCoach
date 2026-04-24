using System.Text.Json;
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
            .ToListAsync(cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task<BugReport?> GetReportByIdAsync(int id, CancellationToken cancellationToken = default)
    {
        return await db.BugReports
            .AsNoTracking()
            .FirstOrDefaultAsync(b => b.Id == id, cancellationToken)
            .ConfigureAwait(false);
    }

    public async Task AddReportAsync(GenerateRequest request, GenerateContentResult result,
        CancellationToken cancellationToken = default)
    {
        var report = new BugReport
        {
            Code = request.Code,
            ErrorMessage = request.ErrorMessage,
            AiResponseJson = JsonSerializer.Serialize(result),
            CreatedAt = DateTime.UtcNow
        };

        await db.BugReports.AddAsync(report, cancellationToken);
        await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
    }

    public async Task DeleteReportAsync(int id, CancellationToken cancellationToken = default)
    {
        var report = await db.BugReports.FindAsync([id], cancellationToken).ConfigureAwait(false);
        if (report != null)
        {
            db.BugReports.Remove(report);
            await db.SaveChangesAsync(cancellationToken).ConfigureAwait(false);
        }
    }
}