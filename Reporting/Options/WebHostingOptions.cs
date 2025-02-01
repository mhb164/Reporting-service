namespace Tizpusoft.Reporting.Options;

public class WebHostingOptions
{
    public const string ConfigName = "WebHosting";

    public int? Port { get; set; }
    public int? SslPort { get; set; }
    public string? UrlPathPrefix { get; set; }
}