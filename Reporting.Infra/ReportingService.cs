using Microsoft.Extensions.Logging;

namespace Tizpusoft.Reporting;

public class ReportingService : IReportingService
{
    private readonly ILogger? _logger;
    private readonly IReportingRepository _repository;

    public ReportingService(ILogger<ReportingService>? logger, IReportingRepository repository)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));

        _repository.InitialAsync().Wait();
    }
}
