using System.Diagnostics;

namespace Tizpusoft;

public static partial class Aid
{
    public const ulong OneKB = 1024;
    public const ulong OneMB = 1024 * 1024;
    public const ulong OneGB = 1024 * 1024 * 1024;

    public static string GetSystemMemoryText()
    {
        var memoryStatusEx = new NativeMethods.MEMORYSTATUSEX();
        if (!NativeMethods.GlobalMemoryStatusEx(memoryStatusEx))
            return "Not Available!";

        var physicalMemory = (decimal)memoryStatusEx.ullTotalPhys / OneGB;
        var freeMemory = (decimal)memoryStatusEx.ullAvailPhys / OneGB;
        var usedMemory = physicalMemory - freeMemory;

        return $"{memoryStatusEx.dwMemoryLoad}% in use ({usedMemory:N1} GB of {physicalMemory:N1} GB)";
    }

    public static decimal ProcessUsageInKB => (decimal)Process.GetCurrentProcess().WorkingSet64 / OneKB;
    public static string GetProcessUsageText()
    {
        var workingSet64 = (decimal)Process.GetCurrentProcess().WorkingSet64;

        var processUsageInKB = workingSet64 / OneKB;
        var processUsageInMB = workingSet64 / OneMB;
        return $"{processUsageInMB:N0} MB  ({processUsageInKB} KB)";
    }
}
