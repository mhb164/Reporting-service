using System.Text;
using Tizpusoft.Reporting.Auth;
using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

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

            if (httpContext?.Items["ClientName"] is string clientName)
                info.AppendLine($" - Api Client '{clientName}' authenticated");
            else
                info.AppendLine($" - Unknown Api Client!");

            if (httpContext?.Items["ClientUser"] is ClientUser clientUser)
                info.AppendLine($" - '{clientUser.Name}' authenticated {clientUser.IssuedAt:yyyy-MM-dd HH:mm:ss zzz} by {clientUser.Issuer}[{clientUser.Audience}] until {clientUser.ValidTo:yyyy-MM-dd HH:mm:ss zzz} ({clientUser.ValidTo - DateTime.UtcNow})");
            else
                info.AppendLine($" - Unknown User!");

            return Results.Content(info.ToString(), "text/plain");
        }).WithMetadata(new PublicMetadata());

        endpoints.MapGet("/time",
            () => Results.Json(new { Time = DateTime.Now }))
            .WithMetadata(new PublicMetadata());

        endpoints.MapGet("/latest", static async (HttpContext httpContext, ILastReportService service, CancellationToken cancellationToken) =>
        {
            var latestDetails = await service.GetLatestDetailsAsync();

            var report = new StringBuilder();
            report.AppendLine($"{Aid.ProductName} v{Aid.AppInformationalVersion} {DateTime.Now:yyyy-MM-dd HH:mm:ss.fff zzz} for {httpContext?.Connection?.RemoteIpAddress}");
            report.AppendLine($"System Memory: {Aid.GetSystemMemoryText()}, Process Usage: {Aid.GetProcessUsageText()}");
            if (httpContext?.Items["ClientUser"] is ClientUser clientUser)
                report.AppendLine($" - '{clientUser.Name}' authenticated {clientUser.IssuedAt:yyyy-MM-dd HH:mm:ss zzz} by {clientUser.Issuer}[{clientUser.Audience}] until {clientUser.ValidTo:yyyy-MM-dd HH:mm:ss zzz} ({clientUser.ValidTo - DateTime.UtcNow})");
            else
                report.AppendLine($" - Unknown User!");
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
        endpoints.MapPost("/api/register", 
            static async (RegisterReportRequest request, IReportingService service, CancellationToken cancellationToken) 
                => Map(await service.RegisterAsync(request, cancellationToken)))
            .WithMetadata(new PrivateMetadata(apiClientPermitted: true));

        endpoints.MapGet("/api/latest", 
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
