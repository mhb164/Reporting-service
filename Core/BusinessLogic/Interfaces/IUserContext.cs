using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting.Interfaces;

public interface IUserContext
{
    ClientUser? User { get; }
}
