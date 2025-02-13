using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;
using Tizpusoft.Reporting.Dto;
using Tizpusoft.Reporting.Interfaces;
using Tizpusoft.Reporting.Model;

namespace Tizpusoft.Reporting;

public class LastReportService : ILastReportService
{
    private readonly ReaderWriterLockSlim _changeLock = new ReaderWriterLockSlim();
    private readonly ILogger? _logger;
    private readonly IServiceProvider _serviceProvider;

    private readonly Dictionary<string, ReportDetail> _lastDetail = new();

    public LastReportService(ILogger<LastReportService>? logger, IServiceProvider serviceProvider)
    {
        _logger = logger;
        _serviceProvider = serviceProvider;

        Task.Run(Rebuild);
    }

    private readonly object _rebuildLock = new object();
    private bool _rebuilding = false;

    private async Task<ServiceResult> Rebuild()
    {
        lock (_rebuildLock)
        {
            if (_rebuilding)
            {
                _logger?.LogWarning("[LAST] Rebuild requested, but is in progress!");
                return ServiceResult.BadRequest($"Rebuild in progress!");
            }

            _rebuilding = true;
        }

        _logger?.LogInformation("[LAST] Start rebuild requested.");
        new Thread(RebuildUnsafe) { IsBackground = true, Name = "Rebuild" }.Start();
        return ServiceResult.Success();
    }

    private void RebuildUnsafe()
    {
        _logger?.LogInformation("[LAST] Rebuild started.");
        var stopwatch = Stopwatch.StartNew();
        try
        {
            var details = default(IEnumerable<ReportDetail>);
            using (var scope = _serviceProvider.CreateScope())
            {
                var repository = scope.ServiceProvider.GetRequiredService<IReportingRepository>();
                details = repository.GetLastDetailsByTopicAsync().Result;
            }

            _changeLock.Write(() =>
            {
                foreach (var item in details)
                    AddWithoutLock(item);
            });

            stopwatch.Stop();
            _logger?.LogInformation("[LAST] Rebuild finished successfully took {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger?.LogError(ex, "[LAST] Rebuild failed took {ElapsedMilliseconds} ms.", stopwatch.ElapsedMilliseconds);
        }
        finally
        {
            lock (_rebuildLock)
                _rebuilding = false;
        }
    }

    private void AddWithoutLock(ReportDetail item)
    {
        var key = $"{item.SourceSection.Source}.{item.SourceSection.Section}.{item.Topic}";
        if (!_lastDetail.TryGetValue(key, out var existing))
        {
            _lastDetail.Add(key, item);
            _logger?.LogTrace("[LAST] Added {Key} {Item}", key, item);
            return;
        }

        if (existing.Time > item.Time)
        {
            _logger?.LogTrace("[LAST] Doesn't changed {Key}", key);
            return;
        }

        _lastDetail[key] = item;
        _logger?.LogTrace("[LAST] Changed {Key} {Item}", key, item);
    }

    public void Update(ReportDetail details)
    {
        Task.Run(() =>
        {
            _changeLock.Write(() => AddWithoutLock(details));
        });
    }

    public async Task<ServiceResult<List<ReportDetailDto>>> GetLatestDetailsAsync()
    {
        await Task.CompletedTask;

        var result = _changeLock.Read(() => _lastDetail.Values.Select(x => x.ToDto()).ToList());
        return ServiceResult.Success(result);
    }
}
