using PseudoFTP.Client.Common;
using RestSharp;

namespace PseudoFTP.Client.Services;

abstract class BaseService
{
    protected readonly IRestClient _client;
    protected readonly Configuration _config;

    protected BaseService(Configuration config, IRestClient client)
    {
        _client = client;
        _config = config;
    }
}