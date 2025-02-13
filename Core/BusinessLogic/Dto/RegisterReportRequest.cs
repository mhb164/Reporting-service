namespace Tizpusoft.Reporting.Dto;

public class RegisterReportRequest
{
    public string? Source { get; set; }
    public string? Section { get; set; }
    public DateTime Time { get; set; }
    public string? Topic { get; set; }
    public string? TraceKey { get; set; }
    public string? Text { get; set; }
}
