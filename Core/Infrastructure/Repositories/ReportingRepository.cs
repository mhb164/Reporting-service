using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Repositories;

public class ReportingRepository : IReportingRepository
{
    private const int MaxRetries = 3;
    private const int RetryDelayStep = 1000;

    private readonly ILogger? _logger;
    private readonly ReportingDbContext _context;

    public ReportingRepository(ILogger<ReportingRepository>? logger, ReportingDbContext context)
    {
        _logger = logger;
        _context = context;
    }

    private async Task<T?> InvokeByRetry<T>(Func<Task<T>> func)
    {
        int retry = 1;
        for (; retry <= MaxRetries; retry++)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                var result = await func();
                await transaction.CommitAsync();

                return result;
            }
            catch (DbUpdateException ex)
            {
                // Check if the exception is due to unique constraint violation
                if (_context.IsUniqueConstraintViolation(ex))
                {
                    _logger?.LogWarning("Unique constraint violation detected. Retrying {Retry}...", retry);
                    _context.ClearChangeTracker();
                    await transaction.RollbackAsync();
                    await Task.Delay(retry * RetryDelayStep);
                }
                else
                {
                    throw; // Re-throw the exception if it's not a unique constraint violation
                }
            }
        }

        return default;
    }

    public async Task<Reporter?> GetReporterAsync(string reporterName, bool create)
    {
        return await InvokeByRetry(async () =>
        {
            var reporter = await _context.Reporters
                .FirstOrDefaultAsync(x => x.Name == reporterName);

            if (reporter is not null)
                return reporter;

            if (!create)
                return null;

            reporter = new Reporter() { Name = reporterName };
            await _context.Reporters.AddAsync(reporter);
            await _context.SaveChangesAsync();

            return reporter;
        });
    }

    public async Task<ReportSourceSection?> GetSourceSectionAsync(string sourceName, string sectionName, bool create)
    {
        return await InvokeByRetry(async () =>
        {
            var source = await _context.SourceSections
                .FirstOrDefaultAsync(x => x.Source == sourceName && x.Section == sectionName);

            if (source is not null)
                return source;

            if (!create)
                return null;

            source = new ReportSourceSection() { Source = sourceName, Section = sectionName };
            await _context.SourceSections.AddAsync(source);
            await _context.SaveChangesAsync();

            return source;
        });
    }

    public async Task<ReportDetail?> AddAsync(ReportDetail details)
    {
        await _context.Details.AddAsync(details);
        await _context.SaveChangesAsync();
        return details;
    }

    public async Task<IEnumerable<ReportDetail>> GetLastDetailsByTopicAsync()
    {
        var groups = await _context.Details
            .Include(x => x.Reporter)
            .Include(x => x.SourceSection)
            .Include(x => x.Reporter)
            .GroupBy(x => x.Topic)
            .Select(group => new
            {
                Name = group.Key,
                MaxValue = group.Max(item => item.Time),
                Item = group.OrderByDescending(item => item.Time).FirstOrDefault()
            })
            .ToListAsync();

        return groups.Select(x => x.Item);
    }
}
