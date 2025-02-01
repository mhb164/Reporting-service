using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tizpusoft.Reporting.Options;

namespace Tizpusoft.Reporting;

public class ReportingSqlContext : ReportingDbContext
{
    public ReportingSqlContext()
    {
    }

    public ReportingSqlContext(ILogger<ReportingDbContext>? logger, IOptions<RepositoryOptions>? options) : base(logger, options)
    {
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        optionsBuilder.UseSqlServer(_options?.ConnectionString);
    }

    public override async Task InitialAsync()
    {
        await Database.MigrateAsync();
    }


}
