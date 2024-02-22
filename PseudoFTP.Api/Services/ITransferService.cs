using PseudoFTP.Model;
using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;

namespace PseudoFTP.Api.Services;

public interface ITransferService
{
    Task<int> TransferAsync(User user, TransferOption option);
    Task<TransferHistoryDto?> GetTransferHistoryAsync(User user, int id);
    Task<IEnumerable<TransferHistoryDto>> GetTransferHistoriesAsync(User user);
}