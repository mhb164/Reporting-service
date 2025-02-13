using System.Text;
using Tizpusoft.Reporting.Auth;
using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Interfaces;

namespace Tizpusoft.Reporting;

public static class EndpointExtensions
{
    private static IResult Map<T>(this ServiceResult<T> result) => result.Code switch
    {
        ServiceResultCode.Success => Results.Json(result.Value),
        _ => ErrorResponse.Generate(result),
    };

    private static IResult Map(this ServiceResult result) => result.Code switch
    {
        ServiceResultCode.Success => Results.Ok(),
        _ => ErrorResponse.Generate(result),
    };

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
        }).WithMetadata(new PublicMetadata());

        endpoints.MapGet("/Latest", static async (HttpContext httpContext, ILastReportService service, CancellationToken cancellationToken) =>
        {
            var latestDetails = await service.GetLatestDetailsAsync();

            var report = new StringBuilder();
            report.AppendLine($"{Aid.ProductName} v{Aid.AppInformationalVersion} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} for {httpContext?.Connection?.RemoteIpAddress}");
            report.AppendLine($"System Memory: {Aid.GetSystemMemoryText()}, Process Usage: {Aid.GetProcessUsageText()}");
            report.AppendLine("");

            if (latestDetails?.Value is not null)
            {
                var bySourceSection = latestDetails.Value.GroupBy(x => $"{x.Source}-{x.Section}");
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
        }).WithMetadata(new PublicMetadata());
        return endpoints;
    }

    public static IEndpointRouteBuilder MapRegisterEndpoints(this IEndpointRouteBuilder endpoints)
    {
        endpoints.MapPost("/api/Register", 
            static async (RegisterReportRequest request, IReportingService service, CancellationToken cancellationToken) 
                => Map(await service.RegisterAsync(request, cancellationToken)))
            .WithMetadata(new PrivateMetadata(apiClientPermitted: true));

        endpoints.MapGet("/api/Latest", 
                async (ILastReportService service, CancellationToken cancellationToken)
                    => Map(await service.GetLatestDetailsAsync()))
            .WithMetadata(new PrivateMetadata(apiClientPermitted: true));

        return endpoints;
    }

    private static async Task<string?> GetApiClientNameAsync(HttpContext httpContext, IApiKeyAuthenticationService service)
    {
        if (!httpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeaderValue))
            return null;

        return await service.GetApiClientNameAsync(apiKeyHeaderValue);
    }
}
