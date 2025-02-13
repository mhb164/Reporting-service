using Microsoft.Extensions.Logging;
using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;
public class ReportingService : IReportingService
{
    private readonly ILogger? _logger;
    private readonly IReportingRepository _repository;
    private readonly ILastReportService _lastReports;
    private readonly IApiContext? _apiContext;

    public ReportingService(ILogger<ReportingService>? logger,  IReportingRepository repository, ILastReportService lastReportService, IApiContext apiContext)
    {
        _logger = logger;
        _repository = repository ?? throw new ArgumentNullException(nameof(repository));
        _lastReports = lastReportService ?? throw new ArgumentNullException(nameof(lastReportService));
        _apiContext = apiContext;
    }

    public async Task<ServiceResult> RegisterAsync(RegisterReportRequest request, CancellationToken cancellationToken)
    {
        if (string.IsNullOrWhiteSpace(_apiContext?.ClientName))
            return ServiceResult.BadRequest("Reporter is not set!");

        if (string.IsNullOrWhiteSpace(request.Source))
            return ServiceResult.BadRequest("Source is not set!");

        if (string.IsNullOrWhiteSpace(request.Section))
            return ServiceResult.BadRequest("Section is not set!");

        if (string.IsNullOrWhiteSpace(request.Topic))
            return ServiceResult.BadRequest("Topic is not set!");

        request.TraceKey = request.TraceKey ?? string.Empty;

        if (string.IsNullOrWhiteSpace(request.Text))
            return ServiceResult.BadRequest("Text is not set!");

        //_unitOfWork.BeginTransaction();
        try
        {
            var reporter = await _repository.GetReporterAsync(_apiContext.ClientName, true);
            var sourceSection = await _repository.GetSourceSectionAsync(request.Source, request.Section, true);

            var details = new ReportDetail()
            {
                ReporterId = reporter.Id,
                SourceSectionId = sourceSection.Id,
                Time = request.Time,
                Topic = request.Topic,
                TraceKey = request.TraceKey,
                Text = request.Text,
            };

            details = await _repository.AddAsync(details);
            //var changeCount = await _unitOfWork.CommitAsync(cancellationToken);
            if (details is null /*|| changeCount <= 0*/)
                throw new InvalidOperationException("Nothing changed!");

            _lastReports.Update(details);
            return ServiceResult.Success();
        }
        catch (Exception ex)
        {
            //_logger?.LogError(ex, "Error on RegisterAsync");
            //return ServiceResponse<ReportDetail>.Failed("Error");

            var trackingId = Guid.NewGuid();
            _logger?.LogError(ex, "[{TrackingId}] Database error while adding MachineType.", trackingId);
            //await _unitOfWork.RollbackAsync();
            return ServiceResult.InternalError(trackingId, "Database error occurred.");
        }
    }
}
