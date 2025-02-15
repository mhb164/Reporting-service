using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Tizpusoft.Reporting.Auth;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Middleware;

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

        var apiKey = context.Request.Headers[ApiKeyHeaderNames].FirstOrDefault();
        apiContext.ClientName = await _apiKeyAuthenticationService.GetApiClientNameAsync(apiKey);
        context.Items["ClientName"] = apiContext.ClientName;

        var accessToken = GetBearerToken(context);
        ((UserContext)userContext).User = GetUser(accessToken);
        context.Items["ClientUser"] = userContext.User;

        var authMetadata = GetAuthMetadata(endpoint);

        if (authMetadata is PublicMetadata || authMetadata is not PrivateMetadata privateAuth)
        {
            await _next(context);
            return;
        }

        if (privateAuth.ApiClientPermitted && !string.IsNullOrWhiteSpace(apiContext.ClientName))
        {
            await _next(context);
            return;
        }

        if (userContext.User is not null)
        {
            await _next(context);
            return;
        }

        context.Response.StatusCode = StatusCodes.Status401Unauthorized;
        return;
    }

    private static AuthMetadata GetAuthMetadata(Endpoint endpoint)
        => endpoint.Metadata?.GetMetadata<AuthMetadata>() ?? PrivateMetadata.Limited;

    private static string? GetBearerToken(HttpContext context)
    {
        var authHeader = context.Request.Headers[HeaderNames.Authorization].FirstOrDefault();

        if (authHeader is null)
            return null;

        if (!authHeader.StartsWith(BearerAuthorizationStart))
            return null;

        return (string?)authHeader[BearerAuthorizationStart.Length..].Trim();
    }

    private ClientUser? GetUser(string? accessToken)
    {
        if (string.IsNullOrWhiteSpace(accessToken))
            return null;

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = _jwt.Issuer,
            ValidateAudience = true,
            ValidAudiences = _jwt.ValidAudiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = [_jwt.SecretKey],
            ValidateLifetime = true
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        try
        {
            tokenHandler.ValidateToken(accessToken, validationParameters, out var validatedToken);
            var jwtSecurityToken = validatedToken as JwtSecurityToken;

            if (jwtSecurityToken == null)
                return null;

            return new ClientUser(name: jwtSecurityToken.Claims.FirstOrDefault(x => x.Type == JwtRegisteredClaimNames.Name)?.Value,
                                  audience: jwtSecurityToken.Audiences.FirstOrDefault(),
                                  issuer: jwtSecurityToken.Issuer,
                                  issuedAt: jwtSecurityToken.IssuedAt,
                                  validTo: jwtSecurityToken.ValidTo);
        }
        catch
        {
            return null;
        }
    }
}