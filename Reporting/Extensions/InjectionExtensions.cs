
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Middleware;
using Tizpusoft.Reporting.Options;
using Tizpusoft.Reporting.Repositories;
using static System.Formats.Asn1.AsnWriter;

namespace Tizpusoft.Reporting;

public static class InjectionExtensions
{
    public static IServiceCollection ProvideServices(
         this IServiceCollection services, ConfigurationManager configuration)
    {
        // Set the JSON serializer options
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = true;
        });

        ConfigureReportingDbContext(services, configuration);

        services.AddScoped<IApiContext, ApiContext>();
        services.AddScoped<IUserContext, UserContext>();

        services.AddSingleton(sp => sp.GetRequiredService<IOptions<List<ApiKeyAuthenticationOptions>>>().Value.Select(x => x.ToModel()));
        services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtOptions>>().Value.ToModel());

        services.AddSingleton<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();        
        services.AddSingleton<ILastReportService, LastReportService>();

        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IReportingRepository, ReportingRepository>();

        return services;
    }

    private static void ConfigureReportingDbContext(IServiceCollection services, ConfigurationManager configuration)
    {
        var options = configuration.GetSection(RepositoryOptions.ConfigName)?.Get<RepositoryOptions>();

        if (options.Provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<ReportingDbContext>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ReportingSqlContext>>();
                return new ReportingSqlContext(logger, options.ConnectionString);
            });
    }

    public static IServiceProvider WarmUp(
         this IServiceProvider services)
    {
        using (var scope = services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<ReportingDbContext>();
            dbContext.InitialAsync().Wait();
        }

        services.GetService<IApiKeyAuthenticationService>();
        services.GetService<ILastReportService>();
     
        return services;
    }
}