using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PseudoFTP.Client.Common;
using PseudoFTP.Client.Utils;
using PseudoFTP.Helper;
using PseudoFTP.Model.Dtos;
using RestSharp;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Client.Services;

class ProfileService : BaseService
{
    private readonly ProfileOptions _options;

    public ProfileService(Configuration config, IRestClient client, ProfileOptions options)
        : base(config, client)
    {
        _options = options;
    }

    public async Task<int> GetProfiles()
    {
        try
        {
            var request = new RestRequest("/api/Profile/GetProfiles");
            request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
            RestResponse response = await _client.ExecuteAsync(request);
            var result = ResponseHelper.GetResult<IEnumerable<ProfileDto>>(response);
            if (result.Any())
            {
                Console.WriteLine("Profiles:");
                foreach (ProfileDto entry in result)
                {
                    Console.WriteLine($"  {entry.Name} -- {entry.Destination}");
                }
            }
            else
            {
                Console.WriteLine("No profiles found.");
            }
        }
        catch (Exception e)
        {
            LogHelper.GetLogger().LogError("Error: {error}", e.Message);
            return 1;
        }

        return 0;
    }

    public async Task<int> AddProfile()
    {
        ILogger logger = LogHelper.GetLogger();
        List<string> param = _options.Add!.ToList();
        string name = param[0];
        string path = param[1];
        if (!Path.IsPathRooted(path))
        {
            logger.LogError("Path must be absolute!");
            return 2;
        }

        try
        {
            var request = new RestRequest("/api/Profile/AddProfile", Method.Post);
            request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
            request.AddHeader("Content-Type", "application/json");
            string body = $"{{\"name\":{JsonConvert.ToString(name)},\"destination\":{JsonConvert.ToString(path)}}}";
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await _client.ExecuteAsync(request);
            ApiResponseDto<ProfileDto> dto = ResponseHelper.GetResponseDto<ProfileDto>(response);
            if (dto.Data == null)
            {
                logger.LogError(dto.Meta.Message);
            }
            else
            {
                logger.LogInformation($"Profile added: {ResponseHelper.FormatResult(dto.Data)}");
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error: {error}", e.Message);
            return 1;
        }

        return 0;
    }

    public async Task<int> DeleteProfile()
    {
        ILogger logger = LogHelper.GetLogger();
        try
        {
            var request = new RestRequest("/api/Profile/DeleteProfile", Method.Delete);
            request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
            request.AddParameter("name", Uri.EscapeDataString(_options.Remove!));
            RestResponse response = await _client.ExecuteAsync(request);
            ApiResponseDto<ProfileDto> result = ResponseHelper.GetResponseDto<ProfileDto>(response);
            if (result.Data == null)
            {
                Console.Error.WriteLine(result.Meta.Message);
            }
            else
            {
                result.Data.Name = result.Data.Name;
                result.Data.Destination = result.Data.Destination;
                logger.LogInformation($"Profile deleted: {ResponseHelper.FormatResult(result.Data)}");
            }
        }
        catch (Exception e)
        {
            logger.LogError("Error: {error}", e.Message);
            return 1;
        }

        return 0;
    }
}