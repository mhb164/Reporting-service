namespace Tizpusoft.Auth;

public class ClientUser
{
    public static readonly string HttpContextKey = "ClientUser";

    public readonly string TokenId;
    public readonly string Name;
    public readonly string Audience;
    public readonly string Issuer;
    public readonly DateTime IssuedAt;
    public readonly DateTime ValidTo;
    public readonly UserPermits Permits;

    public ClientUser(string? tokenId, string? name, string? audience, string? issuer, DateTime? issuedAt, DateTime? validTo, UserPermits permits)
    {
        if (string.IsNullOrWhiteSpace(tokenId))
            throw new ArgumentException($"{nameof(tokenId)} is required!", nameof(tokenId));

        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} is required!", nameof(name));
        if (string.IsNullOrWhiteSpace(audience))
            throw new ArgumentException($"{nameof(audience)} is required!", nameof(audience));
        if (string.IsNullOrWhiteSpace(issuer))
            throw new ArgumentException($"{nameof(issuer)} is required!", nameof(issuer));

        if (issuedAt is null)
            throw new ArgumentNullException(nameof(issuedAt));
        if (validTo is null)
            throw new ArgumentNullException(nameof(validTo));
        if (permits is null)
            throw new ArgumentNullException(nameof(permits));

        TokenId = tokenId;
        Name = name!.Trim();
        Audience = audience!;
        Issuer = issuer!;
        IssuedAt = issuedAt.Value;
        ValidTo = validTo.Value;

        Permits = permits;
    }
}
