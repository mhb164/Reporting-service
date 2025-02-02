namespace Tizpusoft.Reporting;

public interface IApiKeyAuthenticationService
{
    Task<string?> GetApiClientNameAsync(string? apiKey);
}
