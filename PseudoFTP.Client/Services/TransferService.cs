using PseudoFTP.Client.Common;
using RestSharp;

namespace PseudoFTP.Client.Services;

class TransferService : BaseService
{
    private readonly TransferOptions _options;

    public TransferService(Configuration config, IRestClient client, TransferOptions options) : base(config, client)
    {
        _options = options;
    }

    public int GetTransferHistories()
    {
        //var options = new RestClientOptions("http://localhost:6256")
        //{
        //    MaxTimeout = -1,
        //};
        //var client = new RestClient(options);
        //var request = new RestRequest("/api/Transfer/GetTransferHistories", Method.Get);
        //request.AddHeader("X-Credential", "Tony:0xPseudoFTP");
        //RestResponse response = await client.ExecuteAsync(request);
        //Console.WriteLine(response.Content);

        return 0;
    }
}