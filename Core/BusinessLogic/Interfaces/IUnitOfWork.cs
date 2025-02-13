namespace Tizpusoft.Reporting.Interfaces;

public interface IUnitOfWork : IDisposable
{
    void BeginTransaction();
    Task<int> CommitAsync(CancellationToken cancellationToken);
    Task RollbackAsync();
}