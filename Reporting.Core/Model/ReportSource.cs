namespace Tizpusoft.Reporting.Model;

public class ReportSource
{
    public Guid Id { get; set; }
    public string Name { get; set; }

    public List<ReportSection> Sections { get; set; }
}
