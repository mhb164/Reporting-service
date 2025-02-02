using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public interface IReportingService
{
    Task<ServiceResponse<ReportDetail>> RegisterAsync(string reporterName, RegisterReportRequest request, CancellationToken cancellationToken);
}
