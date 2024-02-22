using Microsoft.AspNetCore.Mvc;
using PseudoFTP.Api.Services;
using PseudoFTP.Helper;
using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;
using Tonisoft.AspExtensions.Module;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class ProfileController : BaseController<ProfileController>
{
    private readonly IProfileService _profileService;
    private readonly IUserService _userService;

    public ProfileController(ILogger<ProfileController> logger, IProfileService profileService,
        IUserService userService) : base(logger)
    {
        _profileService = profileService;
        _userService = userService;
    }

    [HttpGet]
    public async Task<ApiResponse> GetProfiles([FromHeader(Name = "X-Credential")] string credential)
    {
        string username = CredentialHelper.GetUsername(credential);
        string password = CredentialHelper.GetPassword(credential);

        User? user = await _userService.GetUserAsync(username, password);
        if (user == null)
        {
            return new UnauthorizedResponse(new UnauthorizedDto());
        }

        IEnumerable<ProfileDto> profiles = await _profileService.GetProfilesAsync(user);

        return new OkResponse(new OkDto(data: profiles));
    }

    [HttpPost]
    public async Task<ApiResponse> AddProfile(
        [FromHeader(Name = "X-Credential")] string credential,
        [FromBody] AddProfileDto profileDto)
    {
        string username = CredentialHelper.GetUsername(credential);
        string password = CredentialHelper.GetPassword(credential);

        User? user = await _userService.GetUserAsync(username, password);
        if (user == null)
        {
            return new UnauthorizedResponse(new UnauthorizedDto());
        }

        ProfileDto? profile = await _profileService.AddProfileAsync(user, profileDto);
        if (profile == null)
        {
            return new OkResponse(new BadDto(1000, "Failed to add profile or name conflict occurs"));
        }

        return new OkResponse(new OkDto(data: profile));
    }

    [HttpDelete]
    public async Task<ApiResponse> DeleteProfile(
        [FromHeader(Name = "X-Credential")] string credential,
        [FromQuery(Name = "id")] int profileId)
    {
        string username = CredentialHelper.GetUsername(credential);
        string password = CredentialHelper.GetPassword(credential);

        User? user = await _userService.GetUserAsync(username, password);
        if (user == null)
        {
            return new UnauthorizedResponse(new UnauthorizedDto());
        }

        try
        {
            await _profileService.DeleteProfileAsync(user, profileId);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Failed to delete profile");
            return new InternalServerErrorResponse(new InternalServerErrorDto(e.Message));
        }

        return new OkResponse(new OkDto());
    }
}