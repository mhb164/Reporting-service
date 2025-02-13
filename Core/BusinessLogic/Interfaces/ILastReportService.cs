using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Interfaces;

public interface ILastReportService
{
    Task<ServiceResult<List<ReportDetailDto>>> GetLatestDetailsAsync();
    void Update(ReportDetail details);
}
