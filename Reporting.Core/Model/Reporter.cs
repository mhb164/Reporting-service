namespace Tizpusoft.Reporting.Model;

public class Reporter
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<ReportDetail> Details { get; set; }
}
