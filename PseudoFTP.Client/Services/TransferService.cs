using Microsoft.Extensions.Logging;
using PseudoFTP.Client.Common;
using PseudoFTP.Client.Utils;
using PseudoFTP.Helper;
using PseudoFTP.Model.Dtos;
using RestSharp;
using Spectre.Console;

namespace PseudoFTP.Client.Services;

class TransferService : BaseService
{
    private readonly TransferOptions _options;

    public TransferService(Configuration config, IRestClient client, TransferOptions options) : base(config, client)
    {
        _options = options;
    }

    public async Task<int> GetTransferHistories()
    {
        ILogger logger = LogHelper.GetLogger();
        try
        {
            var request = new RestRequest("/api/Transfer/GetTransferHistories");
            request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
            RestResponse response = await _client.ExecuteAsync(request);
            var result = ResponseHelper.GetResult<IEnumerable<TransferHistoryDto>>(response);
            Console.WriteLine("Transfer histories:");
            var table = new Table();
            table.AddColumn("Status")
                .AddColumn("Destination")
                .AddColumn("Started")
                .AddColumn("Completed")
                .AddColumn("Duration")
                .AddColumn("Message")
                .AddColumn("Error");

            foreach (TransferHistoryDto history in result)
            {
                table.AddRow(
                    history.Status ? "Success" : "Failed",
                    history.Destination,
                    history.Started.ToString("yyyy-MM-dd HH:mm:ss"),
                    history.Completed?.ToString("yyyy-MM-dd HH:mm:ss") ?? "N/A",
                    history.Duration + "s",
                    history.Message ?? "N/A",
                    history.Error ?? "N/A");
            }

            table.Border = TableBorder.Rounded;
            table.Collapse();
            AnsiConsole.Write(table);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Failed to get transfer histories.");
            return 1;
        }

        return 0;
    }

    public int Transfer()
    {
        ILogger logger = LogHelper.GetLogger();
        if (string.IsNullOrEmpty(_options.Source))
        {
            logger.LogError("Missing source directory or file.");
            return 1;
        }

        if (string.IsNullOrEmpty(_options.Profile) && string.IsNullOrEmpty(_options.Destination))
        {
            logger.LogError("Specify destination directory or profile.");
            return 2;
        }

        // This dto lacks archive.
        TransferDto dto = new() {
            Profile = _options.Profile,
            Destination = _options.Destination,
            Message = _options.Message,
            FtpIgnore = _options.FtpIgnore,
            Overwrite = _options.Overwrite,
            PurgePrevious = _options.PurgePrevious,
            KeepOriginal = _options.KeepOriginal
        };
        string credential = CredentialHelper.GetCredential(_config.Username, _config.Password);

        var agent = new TransferAgent(_client, this, credential);

        logger.LogInformation("Preparing to transfer...");
        TransferHistoryDto? result = new ProgressHandler<TransferHistoryDto?>("Transferring",
            agent.Transfer(_options.Source!, dto, _config.MaxRetry)).AnimatedPerform();
        if (result == null)
        {
            logger.LogError("Failed to transfer.");
            return 1;
        }

        if (result.IsSuccessful)
        {
            logger.LogInformation("Transfer completed.");
        }
        else if (result.IsFailed)
        {
            logger.LogError("Transfer failed: {error}", result.Error);
        }
        else
        {
            logger.LogWarning("Transfer checking timed out. Use `transfer --history` to check for latest status.");
        }

        return 0;
    }

    public async Task<TransferHistoryDto?> GetTransferHistory(int id)
    {
        var request = new RestRequest("/api/Transfer/GetTransferHistory");
        request.AddQueryParameter("id", id.ToString());
        request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
        RestResponse response = await _client.ExecuteAsync(request);
        var result = ResponseHelper.GetResult<TransferHistoryDto>(response);
        return result;
    }
}