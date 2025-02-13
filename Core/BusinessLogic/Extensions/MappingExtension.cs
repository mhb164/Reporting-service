using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public static partial class MappingExtension
{
    public static ReportDetailDto ToDto(this ReportDetail modelItem)
        => new ReportDetailDto(modelItem.Id, modelItem.Time, modelItem.SourceSection?.Source, modelItem.SourceSection?.Section, modelItem.Reporter?.Name, modelItem.Topic, modelItem.TraceKey, modelItem.Text);

  
}
