using Tizpusoft.Reporting.Auth;

namespace Tizpusoft.Reporting.Options;

public class JwtOptions
{
    public const string ConfigName = "Jwt";

    public string? Issuer { get; set; }
    public string? Secret { get; set; }
    public List<string>? ValidAudiences { get; set; }

    public JwtConfig ToModel()
        => new JwtConfig(Issuer, Secret, ValidAudiences);
}