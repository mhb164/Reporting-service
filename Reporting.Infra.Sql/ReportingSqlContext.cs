using Microsoft.Data.SqlClient;
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

    public override bool IsUniqueConstraintViolation(DbUpdateException ex)
    {
        // Check if the exception is due to unique constraint violation
        // Implementation may vary depending on the database provider

        return ex.InnerException is SqlException sqlEx && (sqlEx.Number == 2627 || sqlEx.Number == 2601);
    }


}
