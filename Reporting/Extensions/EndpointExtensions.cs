using System.Text;
using Tizpusoft.Reporting.Dto;

namespace Tizpusoft.Reporting;

public static class EndpointExtensions
{
    public static IEndpointRouteBuilder MapEndpoints(this IEndpointRouteBuilder endpoints)
        => endpoints.MapGeneralEndpoints()
                    .MapRegisterEndpoints();


    public static IEndpointRouteBuilder MapGeneralEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapGet("/info", (HttpContext httpContext) =>
        {
            var info = new StringBuilder();
            info.AppendLine($"{Aid.ProductName} v{Aid.AppInformationalVersion} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} for {httpContext?.Connection?.RemoteIpAddress}");

            return Results.Content(info.ToString(), "text/plain");
        });

        return endpoints;
    }

    public static IEndpointRouteBuilder MapRegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/Register", static async (HttpContext httpContext, RegisterReportRequest request, IApiKeyAuthenticationService apiKeyAuthenticationService, IReportingService service, CancellationToken cancellationToken) =>
        {
            var reporterName = await GetApiClientNameAsync(httpContext, apiKeyAuthenticationService);
            if (reporterName is null)
                return Results.Unauthorized();
           
            var result = await service.RegisterAsync(reporterName, request, cancellationToken);

            if (result.Success)
                return Results.Ok(result.Message);
            else
                return Results.Problem(result.Message);
        });

        endpoints.MapGet("/api/Latest", async (HttpContext httpContext, ILastReportService service, IApiKeyAuthenticationService apiKeyAuthenticationService, CancellationToken cancellationToken) =>
        {
            var reporterName = await GetApiClientNameAsync(httpContext, apiKeyAuthenticationService);
            if (reporterName is null)
                return Results.Unauthorized();

            var result = await service.GetLatestDetailsAsync();

            if (result.Success)
                return Results.Json(result.Data);
            else
                return Results.Problem(result.Message);
        });

        endpoints.MapGet("/Latest", static async (HttpContext httpContext, ILastReportService service, CancellationToken cancellationToken) =>
        {
            var latestDetails = await service.GetLatestDetailsAsync();

            var report = new StringBuilder();
            report.AppendLine($"{Aid.ProductName} v{Aid.AppInformationalVersion} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} for {httpContext?.Connection?.RemoteIpAddress}");
            report.AppendLine($"System Memory: {Aid.GetSystemMemoryText()}, Process Usage: {Aid.GetProcessUsageText()}");
            report.AppendLine("");

            if (latestDetails?.Data is not null)
            {
                var bySourceSection = latestDetails.Data.GroupBy(x => $"{x.Source}-{x.Section}");
                foreach (var group in bySourceSection.OrderBy(x => x.Key))
                {
                    report.AppendLine($"* {group.Key}");
                    foreach (var item in group.OrderBy(x => x.Time))
                    {
                        report.AppendLine($"  - {item.Time:yyyy-MM-dd HH:mm:ss.fff zzz}> {item.Topic}: {item.Text} [Trace: {item.TraceKey}]");
                    }
                }
            }

            return Results.Content(report.ToString(), "text/plain");
        });

        return endpoints;
    }

    private static async Task<string?> GetApiClientNameAsync(HttpContext httpContext, IApiKeyAuthenticationService service)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeaderValue))
            return null;

        return await service.GetApiClientNameAsync(apiKeyHeaderValue);
    }
}
