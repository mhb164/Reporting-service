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

    public abstract bool IsUniqueConstraintViolation(DbUpdateException ex);

    internal void ClearChangeTracker()
    {
        foreach (var entry in ChangeTracker.Entries())
            entry.State = EntityState.Detached;
    }

    /*
    public static bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // Handle SQL Server
        if (ex.InnerException is SqlException sqlEx && sqlEx.Number == 2627)
        {
            return true;
        }

        // Handle SQLite
        if (ex.InnerException is SqliteException sqliteEx && sqliteEx.SqliteErrorCode == 19 && sqliteEx.Message.Contains("UNIQUE"))
        {
            return true;
        }

        // Handle PostgreSQL
        if (ex.InnerException is PostgresException pgEx && pgEx.SqlState == "23505")
        {
            return true;
        }

        // Handle other providers if needed

        return false;
    }    
     */
}
