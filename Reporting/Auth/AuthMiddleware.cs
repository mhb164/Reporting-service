using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Tizpusoft.Reporting.Config;
using Tizpusoft.Reporting.Interfaces;

namespace Tizpusoft.Auth;

public class AuthMiddleware
{
    public static readonly string BearerAuthorizationStart = "Bearer ";
    public static readonly string ApiKeyHeaderNames = "X-API-KEY";

    private readonly JwtConfig _jwt;
    private readonly IApiKeyAuthenticationService _apiKeyAuthenticationService;
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next, JwtConfig jwtConfig, IApiKeyAuthenticationService apiKeyAuthenticationService)
    {
        _jwt = jwtConfig;
        _next = next;
        _apiKeyAuthenticationService = apiKeyAuthenticationService;
    }

    public async Task InvokeAsync(HttpContext context, IApiContext apiContext, IUserContext userContext)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            await _next(context);
            return;
        }

        ((ApiContext)apiContext).Client = await GetApiClientAsync(context);
        context.Items[ClientApi.HttpContextKey] = apiContext.Client;

        var accessToken = GetBearerToken(context);
        ((UserContext)userContext).User = GetUser(accessToken);
        context.Items[ClientUser.HttpContextKey] = userContext.User;

        var authMetadata = GetAuthMetadata(endpoint);

        if (!authMetadata.HasAccess(userContext.User?.Permits, apiContext.Client))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            return;
        }

        await _next(context);
    }

    private async Task<ClientApi?> GetApiClientAsync(HttpContext httpContext)
    {
        var clientIp = GetClientIp(httpContext);
        if (string.IsNullOrWhiteSpace(clientIp))
            return null;

        var apiKey = httpContext.Request.Headers[ApiKeyHeaderNames].FirstOrDefault();

        var clientApiName = await _apiKeyAuthenticationService.GetApiClientNameAsync(apiKey);
        if (string.IsNullOrWhiteSpace(clientApiName))
            return null;

        return new ClientApi(ip: clientIp, name: clientApiName);
    }

    public string? GetClientIp(HttpContext httpContext)
    {
        // Check if the request has the X-Forwarded-For header
        if (httpContext.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedHeader))
        {
            var ipList = forwardedHeader.ToString().Split(',');
            if (ipList.Length > 0)
            {
                return ipList[0].Trim();  // First IP in the list is the real client IP
            }
        }

        // Fallback to RemoteIpAddress if no proxy is used
        return httpContext.Connection.RemoteIpAddress?.ToString();
    }

    private static AuthMetadata GetAuthMetadata(Endpoint endpoint)
       => endpoint.Metadata?.GetMetadata<AuthMetadata>() ?? ApiPermissions.TokenIsEnough;

    private static string? GetBearerToken(HttpContext context)
    {
        var authHeader = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

        if (authHeader is null)
            return null;

        if (!authHeader.StartsWith(BearerAuthorizationStart))
            return null;

        return (string?)authHeader[BearerAuthorizationStart.Length..].Trim();
    }

    public ClientUser? GetUser(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return default;

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(accessToken, _jwt.ValidationParameters, out var validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            if (jwtSecurityToken == null)
                return default;

            var name = jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value;
            if (string.IsNullOrWhiteSpace(name))
                return default;


            if (string.IsNullOrWhiteSpace(jwtSecurityToken.Id))
                return default;

            var permits = new UserPermits(ReadPermits(jwtSecurityToken.Claims));

            return new ClientUser(tokenId: jwtSecurityToken.Id, name: name,
                                  audience: jwtSecurityToken.Audiences.FirstOrDefault(),
                                  issuer: jwtSecurityToken.Issuer,
                                  issuedAt: jwtSecurityToken.IssuedAt,
                                  validTo: jwtSecurityToken.ValidTo,
                                  permits);
        }
        catch
        {
            return default;
        }
    }

    private static IEnumerable<UserPermit> ReadPermits(IEnumerable<Claim> claims)
    {
        foreach (var claim in claims)
        {
            var permit = UserPermit.FromClaim(claim.Type, claim.Value);

            if (permit is null)
                continue;

            yield return permit;
        }
    }
}