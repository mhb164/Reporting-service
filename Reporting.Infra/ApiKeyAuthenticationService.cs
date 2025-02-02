using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Tizpusoft.Reporting.Options;

namespace Tizpusoft.Reporting;

public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
{
    private readonly ILogger? _logger;
    private readonly Dictionary<string, ApiKeyAuthenticationOptions> _apiKeys = new(StringComparer.OrdinalIgnoreCase);

    public ApiKeyAuthenticationService(ILogger<ApiKeyAuthenticationService>? logger, IOptions<List<ApiKeyAuthenticationOptions>>? options)
    {
        _logger = logger;
        if (options?.Value is not null)
            foreach (var item in options.Value)
                Add(item);
    }

    private void Add(ApiKeyAuthenticationOptions item)
    {
        if (string.IsNullOrWhiteSpace(item.ApiKey))
            return;

        item.ApiKey = item.ApiKey.Trim();
        if (_apiKeys.ContainsKey(item.ApiKey))
            return;

        _apiKeys.Add(item.ApiKey, item);
    }

    public async Task<string?> GetApiClientNameAsync(string? apiKey)
    {
        await Task.CompletedTask;
        if (string.IsNullOrWhiteSpace(apiKey))
            return null;

        if (_apiKeys.TryGetValue(apiKey, out var options))
            return options.Name;

        return null;
    }
}
