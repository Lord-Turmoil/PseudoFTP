using Newtonsoft.Json;
using RestSharp;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Client.Utils;

static class ResponseHelper
{
    public static string GetNonNullResponseContent(RestResponse response)
    {
        if (response.StatusCode == 0)
        {
            throw new Exception("Timed out!");
        }

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

    public static ApiResponseDto GetResponseDto(RestResponse response)
    {
        string content = GetNonNullResponseContent(response);
        var dto = JsonConvert.DeserializeObject<ApiResponseDto>(content);
        if (dto == null)
        {
            throw new Exception("Failed to deserialize the response");
        }

        return dto;
    }

    public static ApiResponseDto<TResult> GetResponseDto<TResult>(RestResponse response)
    {
        string content = GetNonNullResponseContent(response);
        var dto = JsonConvert.DeserializeObject<ApiResponseDto<TResult>>(content);
        if (dto == null)
        {
            throw new Exception("Failed to deserialize the response");
        }

        return dto;
    }

    public static TResult? GetNullableResult<TResult>(RestResponse response)
    {
        ApiResponseDto<TResult> dto = GetResponseDto<TResult>(response);
        return dto.Data;
    }

    public static TResult GetResult<TResult>(RestResponse response)
    {
        var result = GetNullableResult<TResult>(response);
        if (result == null)
        {
            throw new Exception("Result suppose to be not null");
        }

        return result;
    }

    public static string FormatResult<TResult>(TResult result)
    {
        return JsonConvert.SerializeObject(result, Formatting.Indented);
    }
}