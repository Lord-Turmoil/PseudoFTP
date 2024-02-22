using Arch.EntityFrameworkCore.UnitOfWork;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PseudoFTP.Helper;
using PseudoFTP.Model;
using PseudoFTP.Model.Database;

namespace PseudoFTP.Api.Tasks;

public class TransferTask : IHostedService, IDisposable
{
    private const int MaxParallel = 5;
    private static readonly object _mutex = new();

    /// <summary>
    ///     Currently affected directories.
    /// </summary>
    private static readonly List<string> _activeDirectories = [];

    private readonly ILogger<TransferTask> _logger;
    private readonly IServiceScopeFactory _serviceScopeFactory;

    public TransferTask(ILogger<TransferTask> logger, IServiceScopeFactory serviceScopeFactory)
    {
        _logger = logger;
        _serviceScopeFactory = serviceScopeFactory;
    }

    public void Dispose()
    {
        // TODO release managed resources here
    }

    public Task StartAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transfer task online!");
        return Task.CompletedTask;
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Transfer task offline!");
        return Task.CompletedTask;
    }

    /// <summary>
    /// Add a new task to the queue.
    /// If the requested path is currently being processed, return false.
    /// </summary>
    /// <param name="path">Requested path.</param>
    /// <returns>Whether able to proceed.</returns>
    private static bool AddNewTask(string path)
    {
        lock (_mutex)
        {
            if (_activeDirectories.Count >= MaxParallel)
            {
                return false;
            }

            if (_activeDirectories.Any(active => PathHelper.IsRelated(path, active)))
            {
                return false;
            }

            _activeDirectories.Add(path);

            return true;
        }
    }

    /// <summary>
    /// Initiate a transfer, and return the transfer ID, which is the history
    /// ID in the database.
    /// </summary>
    /// <param name="option"></param>
    /// <returns></returns>
    public async Task<int> TransferAsync(User user, TransferOption option)
    {
        if (!AddNewTask(option.Destination))
        {
            return -1;
        }

        int id = await InitiateTransfer(user, option);

        TransferImpl(id, option);

        return id;
    }

    private async Task<int> InitiateTransfer(User user, TransferOption option)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IRepository<TransferHistory> repo = unitOfWork.GetRepository<TransferHistory>();
        DateTime now = DateTime.Now;

        EntityEntry<TransferHistory> entity = await repo.InsertAsync(new TransferHistory {
            UserId = user.Id,
            Destination = option.Destination,
            Started = now,
            Message = option.Message
        });
        await unitOfWork.SaveChangesAsync();

        return entity.Entity.Id;
    }

    /// <summary>
    /// Since it is a detached thread, I think it's better to be exception free.
    /// </summary>
    /// <param name="id">The history ID</param>
    /// <param name="option"></param>
    private async void TransferImpl(int id, TransferOption option)
    {
        using IServiceScope scope = _serviceScopeFactory.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
        IRepository<TransferHistory> repo = unitOfWork.GetRepository<TransferHistory>();

        TransferHistory? history = await repo.FindAsync(id);
        if (history == null)
        {
            _logger.LogError("Transfer history not found: {0}", id);
            return;
        }

        try
        {
            TransferHelper.Transfer(option);
            history.Status = true;
        }
        catch (Exception e)
        {
            history.Status = false;
            history.Error = e.Message;
        }
        finally
        {
            history.Completed = DateTime.Now;
        }

        try
        {
            await unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            _logger.LogError("Failed to save transfer history: {0}", e.Message);
        }

        _logger.LogInformation("Transfer completed: {0}", id);
    }
}