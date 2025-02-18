namespace Tizpusoft.Reporting.Config;

public class RepositoryConfig
{
    public readonly string Provider;
    public readonly string ConnectionString;

    public RepositoryConfig(string? provider, string? connectionString)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(provider, nameof(provider));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(connectionString, nameof(connectionString));

        Provider = provider.Trim();
        ConnectionString = connectionString;
    }

    public override string ToString()
        => $"Repository[Provider:{Provider}][ConnectionString:{ConnectionString}]";
}

