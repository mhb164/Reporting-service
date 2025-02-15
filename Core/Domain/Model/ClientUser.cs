namespace Tizpusoft.Reporting.Model;

public class ClientUser
{
    public readonly string Name;
    public readonly string Audience;
    public readonly string Issuer;
    public readonly DateTime IssuedAt;
    public readonly DateTime ValidTo;

    public ClientUser(string? name, string? audience, string? issuer, DateTime? issuedAt, DateTime? validTo)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(audience, nameof(audience));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(issuer, nameof(issuer));
        ArgumentNullException.ThrowIfNull(issuedAt, nameof(issuedAt));
        ArgumentNullException.ThrowIfNull(validTo, nameof(validTo));

        Name = name.Trim();
        Audience = audience;
        Issuer = issuer;
        IssuedAt = issuedAt.Value;
        ValidTo = validTo.Value;
    }
}