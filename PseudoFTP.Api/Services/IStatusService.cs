using PseudoFTP.Model.Data;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Api.Services;

public interface IStatusService
{
    StatusData GetStatus();
}