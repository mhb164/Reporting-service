using System.Security.Claims;
using System.Text;

namespace Tizpusoft.Reporting;
public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        => endpoints.MapGeneralEndpoints()
                    .MapRegisterEndpoints();


    public static IEndpointRouteBuilder MapGeneralEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/info", async (HttpContext httpContext, ILogger<Program> logger) =>
        {
            var info = new StringBuilder();
            info.AppendLine($"{Aid.ProductName} v{Aid.AppInformationalVersion} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} for {httpContext?.Connection?.RemoteIpAddress}");

            await httpContext.Response.WriteAsync(info.ToString());
        });

        //endpoints.MapGet("/Report", static async (HttpContext httpContext, IReportRepository reporting) =>
        //{
        //    var report = new StringBuilder();
        //    report.AppendLine($"{Aid.ProductName} v{Aid.AppInformationalVersion} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} for {httpContext?.Connection?.RemoteIpAddress}");
        //    report.AppendLine($"System Memory: {Aid.GetSystemMemoryText()}, Process Usage: {Aid.GetProcessUsageText()}");
        //    report.AppendLine("");
        //    report.AppendLine(await reporting.GetReportAsync());

        //    await httpContext.Response.WriteAsync(report.ToString());
        //});

        return endpoints;
    }

    public static IEndpointRouteBuilder MapRegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        //endpoints.MapGet("/Wfs/Status", static async (HttpContext httpContext, IWfsService service) =>
        //{
        //    await httpContext.Response.WriteAsync(service.Status);
        //});

        //endpoints.MapGet("/Wfs/Start", async (HttpContext httpContext, IWfsService service) =>
        //{
        //    var result = service.Synchronize();
        //    await httpContext.Response.WriteAsync(result.Message);
        //});

        return endpoints;
    }
}
