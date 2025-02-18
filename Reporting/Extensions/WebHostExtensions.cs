using Microsoft.AspNetCore.Server.HttpSys;
using Tizpusoft.Reporting.Config;

namespace Tizpusoft.Reporting;

public static class WebHostExtensions
{
    public static void ConfigWebHost(this IWebHostBuilder hostBuilder, WebHostingConfig hostingConfig)
    {      
        hostBuilder.UseHttpSys(options =>
        {
            options.Authentication.Schemes = AuthenticationSchemes.None;
            options.Authentication.AllowAnonymous = true;
            options.MaxConnections = null;
            options.MaxRequestBodySize = 30000000;
            options.UrlPrefixes.Add($"http://*:{hostingConfig.Port}/{hostingConfig.UrlPathPrefix}");
            options.UrlPrefixes.Add($"https://*:{hostingConfig.SslPort}/{hostingConfig.UrlPathPrefix}");
        });
    }

}
