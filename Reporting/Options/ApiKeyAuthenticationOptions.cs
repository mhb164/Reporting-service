using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Options;

public class ApiKeyAuthenticationOptions
{
    public const string ConfigName = "ApiKeys";

    public string Name { get; set; }
    public string ApiKey { get; set; }

    public ApiAuthentication ToModel()
      => new ApiAuthentication(Name, ApiKey);
}
