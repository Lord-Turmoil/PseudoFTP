using Newtonsoft.Json;
using PseudoFTP.Client.Common;
using PseudoFTP.Helper;
using PseudoFTP.Model.Data;
using RestSharp;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Client.Services;

class StatusService
{
    private readonly IRestClient _client;
    private readonly Configuration _config;
    private readonly StatusOptions _options;

    public StatusService(Configuration config, StatusOptions options, IRestClient client)
    {
        _config = config;
        _options = options;
        _client = client;
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

    private static string GetNonNullResponseContent(RestResponse response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error response returned: {response.StatusCode}");
        }

        if (response.Content == null)
        {
            throw new Exception($"Response content is null");
        }

        return response.Content;
    }

    private static TResult? GetNullableResult<TResult>(RestResponse response)
    {
        string content = GetNonNullResponseContent(response);
        var dto = JsonConvert.DeserializeObject<ApiResponseDto<TResult>>(content);
        if (dto == null)
        {
            throw new Exception("Failed to deserialize the response");
        }

        return dto.Data;
    }

    private static TResult GetResult<TResult>(RestResponse response)
    {
        var result = GetNullableResult<TResult>(response);
        if (result == null)
        {
            throw new Exception("Result suppose to be not null");
        }

        return result;
    }

    private string FormatResult<TResult>(TResult result)
    {
        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }
}