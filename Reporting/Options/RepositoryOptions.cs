using Tizpusoft.Reporting.Config;

namespace Tizpusoft.Reporting.Options;

public class RepositoryOptions
{
    public const string ConfigName = "Repository";

    public string? Provider { get; set; }
    public string? ConnectionString { get; set; }

    public static RepositoryConfig ToModel(RepositoryOptions? options)
    {
        ArgumentNullException.ThrowIfNull(options);

        return new RepositoryConfig(options.Provider, options.ConnectionString);
    }
}