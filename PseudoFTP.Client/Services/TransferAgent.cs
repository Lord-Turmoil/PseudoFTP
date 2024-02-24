using PseudoFTP.Client.Utils;
using PseudoFTP.Model.Dtos;
using RestSharp;
using SharpCompress.Archives;
using SharpCompress.Archives.Zip;
using SharpCompress.Common;
using SharpCompress.Writers;
using FileNotFoundException = System.IO.FileNotFoundException;

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
        string archivePath = CompressFiles(source);

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

    private string CompressFiles(string source)
    {
        var archive = ZipArchive.Create();
        if (File.Exists(source))
        {
            archive = ZipArchive.Create();
            archive.AddEntry(Path.GetFileName(source), source);
        }
        else if (Directory.Exists(source))
        {
            archive.AddAllFromDirectory(source, "*");
        }
        else
        {
            throw new FileNotFoundException($"No such file or directory: {source}");
        }

        string archivePath = Path.GetTempPath() + ".zip";
        archive.SaveTo(archivePath, new WriterOptions(CompressionType.Deflate));

        return archivePath;
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
            throw new Exception($"Failed to transfer archive: {ResponseHelper.GetResponseDto<int>(response).Meta.Message}");
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