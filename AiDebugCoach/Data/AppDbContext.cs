using AiDebugCoach.Models;
using Microsoft.EntityFrameworkCore;

namespace AiDebugCoach.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<BugReport> BugReports => Set<BugReport>();
}