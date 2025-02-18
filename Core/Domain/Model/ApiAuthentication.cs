namespace Tizpusoft.Reporting.Model;

public class ApiAuthentication
{
    public readonly string Name;
    public readonly string Key;

    public ApiAuthentication(string? name, string? key)
    {
        ArgumentNullException.ThrowIfNullOrWhiteSpace(name, nameof(name));
        ArgumentNullException.ThrowIfNullOrWhiteSpace(key, nameof(key));

        Name = name.Trim();
        Key = key.Trim();
    }

    public override string ToString()
        => $"ApiClient[Name:{Name}][Key:{Key}]";
}
