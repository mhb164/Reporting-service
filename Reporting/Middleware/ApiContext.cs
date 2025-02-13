using Tizpusoft.Reporting.Interfaces;

namespace Tizpusoft.Reporting.Middleware;

public class ApiContext: IApiContext
{
    public string? ClientName { get; set; }
}
