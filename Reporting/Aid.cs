using System.Net;
using System.Reflection;

namespace Tizpusoft;

public static partial class Aid
{
    public static string? AppVersion
        => Assembly.GetEntryAssembly()?.GetName()?.Version?.ToString();

    public static string? AppFileVersion
        => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyFileVersionAttribute>()?.Version;

    public static string? AppInformationalVersion
        => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

    public static string? ProductName
        => Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyProductAttribute>()?.Product;

    public static void ConfigureAppStart()
    {
        System.Globalization.CultureInfo.DefaultThreadCurrentCulture = System.Globalization.CultureInfo.InvariantCulture;

        ThreadPool.SetMaxThreads(32767, 16184);
        ThreadPool.SetMinThreads(2048, 2048);

        ServicePointManager.UseNagleAlgorithm = true;
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.DefaultConnectionLimit = 8092;
    }
}
