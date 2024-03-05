using PseudoFTP.Client.Utils;
using PseudoFTP.Model.Dtos;
using RestSharp;

namespace PseudoFTP.Client.Services;

class TransferAgent
{
    private readonly IRestClient _client;
    private readonly string _credential;
    private readonly TransferService _service;

    public TransferAgent(IRestClient client, TransferService service, string credential)
    {
        _client = client;
        _service = service;
        _credential = credential;
    }

    public async Task<TransferHistoryDto?> Transfer(string source, TransferDto dto, int maxRetry = 10)
    {
        // pack file
        string archivePath = CompressHelper.CompressFiles(source, dto.FtpIgnore);

        try
        {
            // transfer
            int id = await TransferArchive(archivePath, dto);
            if (id == -1)
            {
                throw new Exception("Failed to transfer archive.");
            }

            // tracking
            return await Tracking(id, maxRetry);
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error: {e.Message}");
            return null;
        }
        finally
        {
            File.Delete(archivePath);
        }
    }


    private async Task<int> TransferArchive(string archivePath, TransferDto dto)
    {
        var request = new RestRequest("/api/Transfer/Transfer", Method.Post);
        request.AddHeader("X-Credential", _credential);
        request.AlwaysMultipartFormData = true;
        request.AddParameter("profile", dto.Profile);
        request.AddParameter("destination", dto.Destination);
        request.AddParameter("message", dto.Message);
        request.AddFile("archive", archivePath);
        request.AddParameter("overwrite", dto.Overwrite);
        request.AddParameter("purgePrevious", dto.PurgePrevious);
        request.AddParameter("keepOriginal", dto.KeepOriginal);
        RestResponse response = await _client.ExecuteAsync(request);
        int id = ResponseHelper.GetResult<int>(response);
        if (id < 0)
        {
            throw new Exception(
                $"Failed to transfer archive: {ResponseHelper.GetResponseDto<int>(response).Meta.Message}");
        }

        return id;
    }

    private async Task<TransferHistoryDto?> Tracking(int id, int maxRetry)
    {
        TransferHistoryDto? dto;
        do
        {
            dto = await _service.GetTransferHistory(id);
            if (dto == null)
            {
                throw new Exception("Failed to get transfer history.");
            }
        } while (--maxRetry > 0 && !dto.IsFinished);

        return dto;
    }
}