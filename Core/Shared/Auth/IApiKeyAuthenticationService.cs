namespace Tizpusoft.Auth;

public interface IApiKeyAuthenticationService
{
    Task<string?> GetApiClientNameAsync(string? apiKey);
}
