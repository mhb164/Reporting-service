namespace Tizpusoft.Reporting.Model;

public class ReportSourceSection
{
    public Guid Id { get; set; }
    public string Source { get; set; }
    public string Section { get; set; }

    public List<ReportDetail> Details { get; set; }
}
