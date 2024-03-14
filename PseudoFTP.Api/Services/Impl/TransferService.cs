using Arch.EntityFrameworkCore.UnitOfWork;
using Arch.EntityFrameworkCore.UnitOfWork.Collections;
using AutoMapper;
using PseudoFTP.Api.Tasks;
using PseudoFTP.Model;
using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;
using Tonisoft.AspExtensions.Module;

namespace PseudoFTP.Api.Services.Impl;

public class TransferService : BaseService<TransferService>, ITransferService
{
    private readonly IRepository<TransferHistory> _repo;
    private readonly TransferTask _task;

    public TransferService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<TransferService> logger,
        IRepository<TransferHistory> repo, TransferTask task)
        : base(unitOfWork, mapper, logger)
    {
        _repo = repo;
        _task = task;
    }

    public Task<int> TransferAsync(User user, TransferOption option)
    {
        return _task.TransferAsync(user, option);
    }

    public async Task<TransferHistoryDto?> GetTransferHistoryAsync(User user, int id)
    {
        TransferHistory? history =
            await _repo.GetFirstOrDefaultAsync(predicate: h => h.Id == id && h.UserId == user.Id);
        return history == null ? null : _mapper.Map<TransferHistory, TransferHistoryDto>(history);
    }

    public async Task<IEnumerable<TransferHistoryDto>> GetTransferHistoriesAsync(User user)
    {
        IPagedList<TransferHistory> histories =
            await _repo.GetPagedListAsync(
                h => h.UserId == user.Id,
                o => o.OrderByDescending(h => h.Started),
                pageIndex: 0,
                pageSize: 10);

        return histories.Items.Select(_mapper.Map<TransferHistory, TransferHistoryDto>);
    }
}