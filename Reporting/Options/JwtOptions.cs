using Tizpusoft.Reporting.Config;

namespace Tizpusoft.Reporting.Options;

public class JwtOptions
{
    public const string ConfigName = "Jwt";

    public string? Issuer { get; set; }
    public string? Secret { get; set; }
    public List<string>? ValidAudiences { get; set; }

    public static JwtConfig ToModel(JwtOptions? options)
    {
        ArgumentNullException.ThrowIfNull(options, nameof(options));

        return new JwtConfig(options.Issuer, options.Secret, options.ValidAudiences);
    }
}