namespace Tizpusoft.Reporting.Auth;

public class PrivateMetadata : AuthMetadata
{
    public static readonly PrivateMetadata Limited = new();

    public readonly bool ApiClientPermitted;

    public PrivateMetadata(bool apiClientPermitted = false)
    {
        ApiClientPermitted = apiClientPermitted;
    }
}
