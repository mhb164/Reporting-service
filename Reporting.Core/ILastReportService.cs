using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public interface ILastReportService
{
    Task<ServiceResponse<List<ReportDetailDto>>> GetLatestDetailsAsync();
    void Update(ReportDetail details);
}
