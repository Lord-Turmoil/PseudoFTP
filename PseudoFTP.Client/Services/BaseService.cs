using Newtonsoft.Json;
using PseudoFTP.Client.Common;
using RestSharp;
using Tonisoft.AspExtensions.Response;

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

    protected static string GetNonNullResponseContent(RestResponse response)
    {
        if (!response.IsSuccessStatusCode)
        {
            throw new Exception($"Error response returned: {response.StatusCode}");
        }

        if (response.Content == null)
        {
            throw new Exception("Response content is null");
        }

        return response.Content;
    }

    protected static ApiResponseDto GetResponseDto(RestResponse response)
    {
        string content = GetNonNullResponseContent(response);
        var dto = JsonConvert.DeserializeObject<ApiResponseDto>(content);
        if (dto == null)
        {
            throw new Exception("Failed to deserialize the response");
        }

        return dto;
    }

    protected static ApiResponseDto<TResult> GetResponseDto<TResult>(RestResponse response)
    {
        string content = GetNonNullResponseContent(response);
        var dto = JsonConvert.DeserializeObject<ApiResponseDto<TResult>>(content);
        if (dto == null)
        {
            throw new Exception("Failed to deserialize the response");
        }

        return dto;
    }

    protected static TResult? GetNullableResult<TResult>(RestResponse response)
    {
        ApiResponseDto<TResult> dto = GetResponseDto<TResult>(response);
        return dto.Data;
    }

    protected static TResult GetResult<TResult>(RestResponse response)
    {
        var result = GetNullableResult<TResult>(response);
        if (result == null)
        {
            throw new Exception("Result suppose to be not null");
        }

        return result;
    }

    protected string FormatResult<TResult>(TResult result)
    {
        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }
}