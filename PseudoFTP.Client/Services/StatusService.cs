using PseudoFTP.Client.Common;
using PseudoFTP.Helper;
using PseudoFTP.Model.Data;
using RestSharp;

namespace PseudoFTP.Client.Services;

class StatusService : BaseService
{
    private readonly StatusOptions _options;

    public StatusService(Configuration config, IRestClient client, StatusOptions options)
        : base(config, client)
    {
        _options = options;
    }

    public async Task<int> GetStatus()
    {
        try
        {
            var request = new RestRequest("/api/Status/GetStatus");
            request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
            RestResponse response = await _client.ExecuteAsync(request);
            var result = GetResult<StatusData>(response);
            Console.WriteLine($"Server status: {FormatResult(result)}");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Error: {e.Message}");
            return 1;
        }

        return 0;
    }
}