using Tizpusoft.Auth;

namespace Tizpusoft.Reporting.Middleware;

public class UserContext : IUserContext
{
    public ClientUser? User { get; set; }
}
