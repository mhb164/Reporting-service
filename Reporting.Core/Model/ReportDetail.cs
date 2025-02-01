namespace Tizpusoft.Reporting.Model;

public class ReportDetail
{
    public Guid Id { get; set; }
    public DateTime Time { get; set; }
    public string Topic { get; set; }
    public string TraceKey { get; set; }
    public string Text { get; set; }

    public ReportSection Section { get; set; }

}