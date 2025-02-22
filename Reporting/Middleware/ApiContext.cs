using Tizpusoft.Auth;

namespace Tizpusoft.Reporting.Middleware;

public class ApiContext: IApiContext
{
    public ClientApi? Client { get; set; }
}
