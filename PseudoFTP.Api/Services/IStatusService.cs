using PseudoFTP.Model.Data;

namespace PseudoFTP.Api.Services;

public interface IStatusService
{
    StatusData GetStatus();
}