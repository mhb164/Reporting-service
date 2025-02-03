using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public interface IReportingRepository
{
    Task<Reporter?> GetReporterAsync(string reporterName, bool create);
    Task<ReportSourceSection?> GetSourceSectionAsync(string sourceName, string sectionName, bool create);
    Task<ReportDetail?> AddAsync(ReportDetail details);
    Task<IEnumerable<ReportDetail>> GetLastDetailsByTopicAsync();
}
