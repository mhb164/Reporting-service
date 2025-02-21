using Microsoft.Extensions.Options;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Options;

public class ApiKeyAuthenticationOptions
{
    public const string ConfigName = "ApiKeys";

    public string? Name { get; set; }
    public string? ApiKey { get; set; }


    public static IEnumerable<ApiAuthentication> ToModel(IEnumerable<ApiKeyAuthenticationOptions>? optionsSet)
    {
        if (optionsSet is null)
            yield break;

        foreach (var options in optionsSet)
            yield return ToModel(options);
    }

    public static ApiAuthentication ToModel(ApiKeyAuthenticationOptions options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        return new ApiAuthentication(options.Name, options.ApiKey);
    }
}
