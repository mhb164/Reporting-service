namespace Tizpusoft.Auth;

public class ClientApi
{
    public static readonly string HttpContextKey = "ClientApi";

    public readonly string IP;
    public readonly string Name;

    public ClientApi(string? ip, string? name)
    {
        if (string.IsNullOrWhiteSpace(ip))
            throw new ArgumentException($"{nameof(ip)} is required!", nameof(ip));
        if (string.IsNullOrWhiteSpace(name))
            throw new ArgumentException($"{nameof(name)} is required!", nameof(name));

        IP = ip!;
        Name = name!.Trim();
    }

    public override string ToString()
       => $"ClientApi[IP:{IP}][Name:{Name}]";
}
