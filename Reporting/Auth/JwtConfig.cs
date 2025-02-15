using Microsoft.IdentityModel.Tokens;

namespace Tizpusoft.Reporting.Auth;

public class JwtConfig
{
    public readonly string Issuer;
    public readonly SymmetricSecurityKey SecretKey;
    public readonly IEnumerable<string> ValidAudiences;

    public JwtConfig(string? issuer, string? secret, IEnumerable<string>? validAudiences)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(issuer, nameof(issuer));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(secret, nameof(secret));
        ArgumentNullException.ThrowIfNull(validAudiences, nameof(validAudiences));

        Issuer = issuer.Trim();
        var jwtSecretBytes = System.Text.Encoding.UTF8.GetBytes(secret.Trim());
        SecretKey = new SymmetricSecurityKey(jwtSecretBytes);
        ValidAudiences = validAudiences;
    }
}
