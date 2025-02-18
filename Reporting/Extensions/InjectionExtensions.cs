using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Middleware;
using Tizpusoft.Reporting.Options;
using Tizpusoft.Reporting.Repositories;

namespace Tizpusoft.Reporting;

public static class InjectionExtensions
{
    public static IServiceCollection ProvideServices(
         this IServiceCollection services, ConfigurationManager? configuration)
    {
        services.AddSingleton(JwtOptions.ToModel(configuration?.GetSection(JwtOptions.ConfigName)?.Get<JwtOptions>()));
        services.AddSingleton(ApiKeyAuthenticationOptions.ToModel(configuration?.GetSection(ApiKeyAuthenticationOptions.ConfigName)?.Get<List<ApiKeyAuthenticationOptions>>()));

        services.AddCors();
        // Set the JSON serializer options
        services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(options =>
        {
            options.SerializerOptions.PropertyNameCaseInsensitive = false;
            options.SerializerOptions.PropertyNamingPolicy = null;
            options.SerializerOptions.WriteIndented = true;
        });

        ConfigureDbContext(services, configuration);

        services.AddScoped<IApiContext, ApiContext>();
        services.AddScoped<IUserContext, UserContext>();

        services.AddSingleton<IApiKeyAuthenticationService, ApiKeyAuthenticationService>();        
        services.AddSingleton<ILastReportService, LastReportService>();

        services.AddScoped<IReportingService, ReportingService>();
        services.AddScoped<IReportingRepository, ReportingRepository>();

        return services;
    }

    private static void ConfigureDbContext(IServiceCollection services, ConfigurationManager? configuration)
    {
        var repositoryConfig = RepositoryOptions.ToModel(configuration?.GetSection(RepositoryOptions.ConfigName)?.Get<RepositoryOptions>());

        if (repositoryConfig.Provider.Equals("SqlServer", StringComparison.OrdinalIgnoreCase))
            services.AddScoped<ReportingDbContext>(provider =>
            {
                var logger = provider.GetRequiredService<ILogger<ReportingSqlContext>>();
                return new ReportingSqlContext(logger, repositoryConfig.ConnectionString);
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