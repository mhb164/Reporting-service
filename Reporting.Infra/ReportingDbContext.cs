using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public abstract class ReportingDbContext : DbContext
{
    protected readonly string _connectionString;
    protected readonly ILogger? _logger;

    public ReportingDbContext() { }
    public ReportingDbContext(ILogger<ReportingDbContext>? logger, string? connectionString) : base()
    {
        _logger = logger;
        _connectionString = connectionString ?? throw new ArgumentNullException(nameof(connectionString));
    }

    public DbSet<Reporter> Reporters { get; set; }
    public DbSet<ReportSource> Sources { get; set; }
    public DbSet<ReportSection> Sections { get; set; }
    public DbSet<ReportDetail> Details { get; set; }

    public abstract Task InitialAsync();
}
