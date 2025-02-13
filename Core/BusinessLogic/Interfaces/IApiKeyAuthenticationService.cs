namespace Tizpusoft.Reporting.Interfaces;

public interface IApiKeyAuthenticationService
{
    Task<string?> GetApiClientNameAsync(string? apiKey);
}
