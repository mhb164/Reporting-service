using Serilog;
using Serilog.Events;
using Tizpusoft.Reporting.Options;

namespace Tizpusoft.Reporting;

public static class ConfigExtensions
{
    public static bool Load(this ConfigurationManager configuration, IWebHostEnvironment environment)
    {
        var configFilename = configuration.GetSection("ConfigFilename")?.Value ?? string.Empty;
        var configFilePath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, configFilename));
        if (!File.Exists(configFilePath))
            throw new InvalidOperationException($"Wrong configuration in {configFilePath}");

        var configJsonBytes = File.ReadAllBytes(configFilePath);
        configuration.AddJsonStream(new MemoryStream(configJsonBytes));
        return true;
    }

    public static void ConfigureLogger(this IHostBuilder hostBuilder, IConfiguration configuration)
    {
        var loggingDirectory = configuration.GetSection($"{LoggingOptions.ConfigName}:Directory")?.Value ?? string.Empty;

        var resolvedLoggingDirectory = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, loggingDirectory));
        if (!Directory.Exists(resolvedLoggingDirectory))
            Directory.CreateDirectory(resolvedLoggingDirectory);

        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
            .WriteTo.File($"{resolvedLoggingDirectory}\\Logs-.log",
                          rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{Scope} {Message:lj}{NewLine}{Exception}",
                          rollOnFileSizeLimit: true, fileSizeLimitBytes: 10 * 1024 * 1024, shared: true, retainedFileCountLimit: 180)
            .WriteTo.File($"{resolvedLoggingDirectory}\\Errors-.log",
                          restrictedToMinimumLevel: LogEventLevel.Error,
                          rollingInterval: RollingInterval.Day,
                          outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}]{Scope} {Message:lj}{NewLine}{Exception}",
                          rollOnFileSizeLimit: true, fileSizeLimitBytes: 10 * 1024 * 1024, shared: true, retainedFileCountLimit: null)
            .WriteTo.Console(outputTemplate: "[{Timestamp:HH:mm:ss} {Level:u3}]{Scope} {Message:lj}{NewLine}{Exception}")
            .CreateLogger();

        hostBuilder.UseSerilog();
    }

    public static IServiceCollection ProvideConfigs(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<RepositoryOptions>(configuration.GetSection(RepositoryOptions.ConfigName));

        return services;
    }
}