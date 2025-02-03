using Microsoft.Extensions.Logging;
using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;
public class ReportingService : IReportingService
{
    private readonly ILogger? _logger;
    private readonly IReportingRepository _repository;
    private readonly ILastReportService _lastReports;

    public ReportingService(ILogger<ReportingService>? logger, IReportingRepository repository, ILastReportService lastReportService)
    {
        _logger = logger;
        _repository = repository;
        _lastReports = lastReportService;
    }

    public async Task<ServiceResponse<ReportDetail>> RegisterAsync(string reporterName, RegisterReportRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(reporterName))
            return ServiceResponse<ReportDetail>.Failed("Reporter is not set!");

        if (string.IsNullOrWhiteSpace(request.Source))
            return ServiceResponse<ReportDetail>.Failed("Source is not set!");

        if (string.IsNullOrWhiteSpace(request.Section))
            return ServiceResponse<ReportDetail>.Failed("Section is not set!");

        if (string.IsNullOrWhiteSpace(request.Topic))
            return ServiceResponse<ReportDetail>.Failed("Topic is not set!");

        request.TraceKey = request.TraceKey ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.Text))
            return ServiceResponse<ReportDetail>.Failed("Text is not set!");

        var reporter = await _repository.GetReporterAsync(reporterName, true);
        var sourceSection = await _repository.GetSourceSectionAsync(request.Source, request.Section, true);

        var details = new ReportDetail()
        {
            Reporter = reporter,
            SourceSection = sourceSection,
            Time = request.Time,
            Topic = request.Topic,
            TraceKey = request.TraceKey,
            Text = request.Text,
        };

        details = await _repository.AddAsync(details);
        if (details is null)
            return ServiceResponse<ReportDetail>.Failed("Error");

        _lastReports.Update(details);
        return ServiceResponse<ReportDetail>.Ok(details, "OK");
    }
}
