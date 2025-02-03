using Microsoft.EntityFrameworkCore;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public class ReportingRepository : IReportingRepository
{
    private readonly ReportingDbContext _context;

    public ReportingRepository(ReportingDbContext context)
    {
        _context = context;
    }

    public async Task<Reporter?> GetReporterAsync(string reporterName, bool create)
    {
        var reporter = await _context.Reporters.SingleOrDefaultAsync(x => x.Name == reporterName);

        if (reporter is not null)
            return reporter;

        if (!create)
            return null;

        reporter = new Reporter() { Name = reporterName };
        await _context.Reporters.AddAsync(reporter);
        await _context.SaveChangesAsync();

        return reporter;
    }

    public async Task<ReportSourceSection?> GetSourceSectionAsync(string sourceName, string sectionName, bool create)
    {
        var source = await _context.SourceSections
            .SingleOrDefaultAsync(x => x.Source == sourceName && x.Section == sectionName);

        if (source is not null)
            return source;

        if (!create)
            return null;

        source = new ReportSourceSection() { Source = sourceName, Section = sectionName };
        await _context.SourceSections.AddAsync(source);
        await _context.SaveChangesAsync();

        return source;
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
