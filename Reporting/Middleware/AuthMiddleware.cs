using Microsoft.AspNetCore.Http;
using Microsoft.Net.Http.Headers;
using System.IdentityModel.Tokens.Jwt;
using Tizpusoft.Reporting.Auth;
using Tizpusoft.Reporting.Interfaces;

namespace Tizpusoft.Reporting.Middleware;

public class AuthMiddleware
{
    public static readonly string BearerAuthorizationStart = "Bearer ";
    public static readonly string ApiKeyHeaderNames = "X-API-KEY";

    private readonly IApiKeyAuthenticationService _apiKeyAuthenticationService;
    private readonly RequestDelegate _next;

    public AuthMiddleware(RequestDelegate next, IApiKeyAuthenticationService apiKeyAuthenticationService)
    {
        _next = next;
        _apiKeyAuthenticationService = apiKeyAuthenticationService;
    }

    public async Task InvokeAsync(HttpContext context, IApiContext apiContext)
    {
        var endpoint = context.GetEndpoint();
        if (endpoint is null)
        {
            await _next(context);
            return;
        }

        var authMetadata = GetAuthMetadata(endpoint);

        // Skip authentication for public endpoints
        if (authMetadata is PublicMetadata || !(authMetadata is PrivateMetadata privateAuth))
        {
            await _next(context);
            return;
        }

        if (privateAuth.ApiClientPermitted)
        {
            var apiKey = context.Request.Headers[ApiKeyHeaderNames].FirstOrDefault();
            apiContext.ClientName = await _apiKeyAuthenticationService.GetApiClientNameAsync(apiKey);
            if (!string.IsNullOrWhiteSpace(apiContext.ClientName))
            {
                context.Items["ClientName"] = apiContext.ClientName;
                await _next(context);
                return;
            }
        }

        // Check for Authorization header
        var token = GetBearerToken(context);
        if (string.IsNullOrWhiteSpace(token) || !IsValidToken(token))
        {
            context.Response.StatusCode = StatusCodes.Status401Unauthorized;
            await context.Response.WriteAsync("Unauthorized: Invalid or missing token.");
            return;
        }

        // If valid, continue to the next middleware
        await _next(context);
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

    private bool IsValidToken(string token)
    {
        var handler = new JwtSecurityTokenHandler();
        try
        {
            var jwtToken = handler.ReadJwtToken(token);
            return jwtToken != null; // You can add more token validation logic here if needed
        }
        catch
        {
            return false;
        }
    }
}