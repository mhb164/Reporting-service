namespace Tizpusoft.Reporting.Model;

public class ReportSection
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public ReportSource Source { get; set; }
    public List<ReportDetail> Details { get; set; }
}
