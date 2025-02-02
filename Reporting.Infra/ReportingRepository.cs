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

    public async Task<ReportSection?> GetSectionAsync(string sourceName, string sectionName, bool create)
    {
        var source = await GetSourceAsync(sourceName, create);

        if (source is null)
            return null;

        var section = source.Sections.SingleOrDefault(x => x.Name == sectionName);

        if (section is not null)
            return section;

        if (!create)
            return null;

        section = new ReportSection() { Name = sectionName, Details = new List<ReportDetail>() };
        source.Sections.Add(section);
        await _context.SaveChangesAsync();

        return section;
    }

    private async Task<ReportSource?> GetSourceAsync(string sourceName, bool create)
    {
        var source = await _context.Sources
            .Include(x => x.Sections)
            .SingleOrDefaultAsync(x => x.Name == sourceName);

        if (source is not null)
            return source;

        if (!create)
            return null;

        source = new ReportSource() { Name = sourceName, Sections = new List<ReportSection>() };
        await _context.Sources.AddAsync(source);
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
            .Include(x => x.Section)
            .Include(x => x.Section.Source)
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
