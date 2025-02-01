using Microsoft.AspNetCore.Server.HttpSys;
using Tizpusoft.Reporting.Options;

namespace Tizpusoft.Reporting;

public static class WebHostExtensions
{
    public const string DefaultUrlPathPrefix = "Reporting";

    public static void ConfigWebHost(this IWebHostBuilder hostBuilder, WebHostingOptions hostingOptions)
    {
        if (hostingOptions is null)
            return;
        var port = hostingOptions?.Port ?? 80;
        var sslPort = hostingOptions?.SslPort ?? 443;
        var urlPathPrefix = hostingOptions?.UrlPathPrefix ?? DefaultUrlPathPrefix;

        hostBuilder.UseHttpSys(options =>
        {
            options.Authentication.Schemes = AuthenticationSchemes.None;
            options.Authentication.AllowAnonymous = true;
            options.MaxConnections = null;
            options.MaxRequestBodySize = 30000000;
            options.UrlPrefixes.Add($"http://*:{port}/{urlPathPrefix}");
            options.UrlPrefixes.Add($"https://*:{sslPort}/{urlPathPrefix}");
        });
    }

}
