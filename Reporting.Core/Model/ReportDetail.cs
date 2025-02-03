﻿using Tizpusoft.Reporting.Dto;

namespace Tizpusoft.Reporting.Model;

public class ReportDetail
{
    public Guid Id { get; set; }
    public DateTime Time { get; set; }
    public string Topic { get; set; }
    public string TraceKey { get; set; }
    public string Text { get; set; }

    public ReportSourceSection SourceSection { get; set; }
    public Reporter Reporter { get; set; }


    public override string ToString() => $"{Time:yyyy-MM-dd HH:mm:ss.fff zzz} [Topic:{Topic}] [TraceKey:{TraceKey}] [Text:{Text}]";

    public ReportDetailDto ToDto()
        => new ReportDetailDto(Id, Time, SourceSection?.Source, SourceSection?.Section, Reporter?.Name, Topic, TraceKey, Text);
}
