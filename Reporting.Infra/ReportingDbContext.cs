using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Tizpusoft.Reporting.Model;
using static System.Net.Mime.MediaTypeNames;

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

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Reporter>()
          .HasIndex(x => x.Name)
          .IsUnique();

        modelBuilder.Entity<ReportSourceSection>()
          .HasIndex(x => new { x.Source, x.Section })
          .IsUnique();
    }

    public DbSet<Reporter> Reporters { get; set; }
    public DbSet<ReportSourceSection> SourceSections { get; set; }
    public DbSet<ReportDetail> Details { get; set; }

    public abstract Task InitialAsync();
}
