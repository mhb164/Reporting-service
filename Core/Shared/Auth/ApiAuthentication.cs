namespace Tizpusoft.Auth;

public class ApiAuthentication
{
    public readonly string Name;
    public readonly string Key;

    public ApiAuthentication(string? name, string? key)
    {
        if (string.IsNullOrWhiteSpace(key))
            throw new ArgumentException($"{nameof(key)} is required!", nameof(key));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} is required!", nameof(name));

        Name = name.Trim();
        Key = key.Trim();
    }

    public override string ToString()
        => $"ApiClient[Name:{Name}][Key:{Key}]";
}
