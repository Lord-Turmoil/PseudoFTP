using PseudoFTP.Client.Common;
using PseudoFTP.Client.Utils;
using PseudoFTP.Helper;
using PseudoFTP.Model;
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
            Console.WriteLine(e.Message);
            return 1;
        }

        return 0;
    }

    public int Transfer()
    {
        if (string.IsNullOrEmpty(_options.Source))
        {
            Console.Error.WriteLine("Missing source directory or file.");
            return 1;
        }
        if (string.IsNullOrEmpty(_options.Profile) && string.IsNullOrEmpty(_options.Destination))
        {
            Console.Error.WriteLine("Specify destination directory or profile.");
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

        Console.WriteLine("Preparing to transfer...");
        TransferHistoryDto? result = new ProgressHandler<TransferHistoryDto?>("Transferring",
            agent.Transfer(_options.Source!, dto, _config.MaxRetry)).AnimatedPerform();
        if (result == null)
        {
            Console.Error.WriteLine("Failed to transfer.");
            return 1;
        }

        if (result.IsSuccessful)
        {
            Console.WriteLine("Transfer completed.");
        }
        else if (result.IsFailed)
        {
            Console.Error.WriteLine("Transfer failed.");
            Console.Error.WriteLine(result.Error);
        }
        else
        {
            Console.Error.WriteLine("Transfer timed out. Use `transfer --history` to check for latest status.");
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