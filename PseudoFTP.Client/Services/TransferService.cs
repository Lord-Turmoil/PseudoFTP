using PseudoFTP.Client.Common;
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
        var request = new RestRequest("/api/Transfer/GetTransferHistories");
        request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
        RestResponse response = await _client.ExecuteAsync(request);
        var result = GetResult<IEnumerable<TransferHistoryDto>>(response);
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

        return 0;
    }
}