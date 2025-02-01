using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tizpusoft.Reporting.Model;
using Tizpusoft.Reporting.Options;

namespace Tizpusoft.Reporting;

public abstract class ReportingDbContext : DbContext, IReportingRepository
{
    protected readonly RepositoryOptions _options;
    protected readonly ILogger? _logger;

    public ReportingDbContext() { }
    public ReportingDbContext(ILogger<ReportingDbContext>? logger,
            IOptions<RepositoryOptions>? options): base()
    {
        _logger = logger;
        _options = options!.Value!;
    }

    public DbSet<ReportSource> Sources { get; set; }
    public DbSet<ReportSection> Sections { get; set; }
    public DbSet<ReportDetail> Details { get; set; }

    public abstract Task InitialAsync();
}
