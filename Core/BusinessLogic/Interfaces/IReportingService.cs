using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Interfaces;

public interface IReportingService
{
    Task<ServiceResult> RegisterAsync(RegisterReportRequest request, CancellationToken cancellationToken);
}
