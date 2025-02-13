using Microsoft.Extensions.Logging;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public class ApiKeyAuthenticationService : IApiKeyAuthenticationService
{
    private readonly ILogger? _logger;
    private readonly Dictionary<string, ApiAuthentication> _apiKeys = new(StringComparer.OrdinalIgnoreCase);

    public ApiKeyAuthenticationService(ILogger<ApiKeyAuthenticationService>? logger, IEnumerable<ApiAuthentication>? apiAuthentications)
    {
        _logger = logger;
        if (apiAuthentications is not null)
            foreach (var item in apiAuthentications)
                Add(item);
    }

    private void Add(ApiAuthentication item)
    {
        if (string.IsNullOrWhiteSpace(item.Key))
            return;

        if (_apiKeys.ContainsKey(item.Key))
            return;

        _apiKeys.Add(item.Key, item);
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
