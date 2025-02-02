using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Tizpusoft.Reporting;

public class ReportingSqlContext : ReportingDbContext
{
    public ReportingSqlContext()
    {
    }

    public ReportingSqlContext(ILogger<ReportingSqlContext>? logger, string? connectionString) : base(logger, connectionString)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_connectionString);
    }

    public override async Task InitialAsync()
    {
        await Database.MigrateAsync();
    }


}
