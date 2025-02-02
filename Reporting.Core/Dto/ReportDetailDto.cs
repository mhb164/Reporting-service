namespace Tizpusoft.Reporting.Dto
{
    public record ReportDetailDto(
        Guid Id, DateTime Time, 
        string Source, string Section, string Reporter, 
        string Topic, string TraceKey, string Text);
}
