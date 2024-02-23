using Newtonsoft.Json;
using PseudoFTP.Client.Common;
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
            var result = GetResult<IEnumerable<ProfileDto>>(response);
            Console.WriteLine($"Profiles: {FormatResult(result)}");
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error: {e.Message}");
            return 1;
        }

        return 0;
    }

    public async Task<int> AddProfile()
    {
        List<string> param = _options.Add!.ToList();
        string name = param[0];
        string path = param[1];
        if (!Path.IsPathRooted(path))
        {
            Console.Error.WriteLine("Error: Path must be absolute!");
            return 2;
        }

        try
        {
            var request = new RestRequest("/api/Profile/AddProfile", Method.Post);
            request.AddHeader("X-Credential", CredentialHelper.GetCredential(_config.Username, _config.Password));
            request.AddHeader("Content-Type", "application/json");
            string body = $"{{\"name\":{JsonConvert.ToString(name)},\"destination\":{JsonConvert.ToString(path)}}}";
            Console.WriteLine($"Body: {body}");
            request.AddStringBody(body, DataFormat.Json);
            RestResponse response = await _client.ExecuteAsync(request);
            ApiResponseDto<ProfileDto> dto = GetResponseDto<ProfileDto>(response);
            if (dto.Data == null)
            {
                Console.Error.WriteLine(dto.Meta.Message);
            }
            else
            {
                Console.WriteLine($"Profile added: {FormatResult(dto.Data)}");
            }
        }
        catch (Exception e)
        {
            Console.Error.WriteLine($"Error: {e.Message}");
            return 1;
        }

        return 0;
    }

    public int RemoveProfile()
    {
        return 0;
    }
}