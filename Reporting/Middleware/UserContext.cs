using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Middleware;

public class UserContext : IUserContext
{
    public ClientUser? User { get; set; }
}
