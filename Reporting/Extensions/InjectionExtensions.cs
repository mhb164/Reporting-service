
namespace Tizpusoft.Reporting;

public static class InjectionExtensions
{
    public static IServiceCollection ProvideServices(
         this IServiceCollection services)
    {
        // Set the JSON serializer options
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = true;
        });

        services.AddSingleton<IReportingService, ReportingService>();
        services.AddSingleton(typeof(IReportingRepository), GetReportingRepositoryType());



        return services;
    }

    private static Type GetReportingRepositoryType()
    {
        return typeof(ReportingSqlContext);
    }

    public static IServiceProvider WarmUp(
         this IServiceProvider services)
    {
        services.GetService<IReportingService>();

        return services;
    }
}