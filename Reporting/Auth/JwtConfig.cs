using Microsoft.IdentityModel.Tokens;

namespace Tizpusoft.Auth;

public class JwtConfig
{
    public readonly string Issuer;
    public readonly SymmetricSecurityKey SecretKey;
    public readonly IEnumerable<string> ValidAudiences;
    public readonly TokenValidationParameters ValidationParameters;

    public JwtConfig(string? issuer, string? secret, IEnumerable<string>? validAudiences)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(issuer);
        ArgumentException.ThrowIfNullOrWhiteSpace(secret);
        ArgumentNullException.ThrowIfNull(validAudiences);
        if (!validAudiences.Any())
            throw new ArgumentException("valid audiences is empty!", nameof(validAudiences));

        Issuer = issuer.Trim();
        var jwtSecretBytes = System.Text.Encoding.UTF8.GetBytes(secret.Trim());
        SecretKey = new SymmetricSecurityKey(jwtSecretBytes);
        ValidAudiences = validAudiences;

        ValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = Issuer,
            ValidateAudience = true,
            ValidAudiences = ValidAudiences,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeys = [SecretKey],
            ValidateLifetime = true
        };
    }
}
