using Microsoft.AspNetCore.Mvc;
using PseudoFTP.Api.Services;
using PseudoFTP.Helper;
using PseudoFTP.Model;
using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;
using Tonisoft.AspExtensions.Module;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class TransferController : BaseController<TransferController>
{
    private readonly IProfileService _profileService;
    private readonly ITransferService _transferService;
    private readonly IUserService _userService;

    public TransferController(ILogger<TransferController> logger, ITransferService transferService,
        IUserService userService, IProfileService profileService)
        : base(logger)
    {
        _transferService = transferService;
        _userService = userService;
        _profileService = profileService;
    }

    [HttpPost]
    public async Task<ApiResponse> Transfer(
        [FromHeader(Name = "X-Credential")] string credential,
        [FromForm] TransferDto dto)
    {
        string username = CredentialHelper.GetUsername(credential);
        string password = CredentialHelper.GetPassword(credential);

        User? user = await _userService.GetUserAsync(username, password);
        if (user == null)
        {
            return new UnauthorizedResponse(new UnauthorizedDto());
        }

        TransferOption? option = await GetTransferOption(user, dto);
        if (option == null)
        {
            return new BadRequestResponse(new BadRequestDto());
        }

        int id = await _transferService.TransferAsync(user, option);
        if (id < 0)
        {
            return new OkResponse(new BadDto(1000, "Server is busy, try again later", data: id));
        }
        return new OkResponse(new OkDto(data: id));
    }

    [HttpGet]
    public async Task<ApiResponse> GetTransferHistory(
        [FromHeader(Name = "X-Credential")] string credential,
        [FromQuery(Name = "id")] int historyId)
    {
        string username = CredentialHelper.GetUsername(credential);
        string password = CredentialHelper.GetPassword(credential);

        User? user = await _userService.GetUserAsync(username, password);
        if (user == null)
        {
            return new UnauthorizedResponse(new UnauthorizedDto());
        }

        TransferHistoryDto? history = await _transferService.GetTransferHistoryAsync(user, historyId);
        if (history == null)
        {
            return new NotFoundResponse(new NotFoundDto());
        }

        return new OkResponse(new OkDto(data: history));
    }

    [HttpGet]
    public async Task<ApiResponse> GetTransferHistories([FromHeader(Name = "X-Credential")] string credential)
    {
        string username = CredentialHelper.GetUsername(credential);
        string password = CredentialHelper.GetPassword(credential);

        User? user = await _userService.GetUserAsync(username, password);
        if (user == null)
        {
            return new UnauthorizedResponse(new UnauthorizedDto());
        }

        IEnumerable<TransferHistoryDto> histories = await _transferService.GetTransferHistoriesAsync(user);
        return new OkResponse(new OkDto(data: histories));
    }

    [NonAction]
    private async Task<TransferOption?> GetTransferOption(User user, TransferDto dto)
    {
        var option = new TransferOption {
            Message = dto.Message,
            Overwrite = dto.Overwrite,
            PurgePrevious = dto.PurgePrevious,
            KeepOriginal = dto.KeepOriginal
        };

        if (dto.IsProfile)
        {
            ProfileDto? profile = await _profileService.GetProfileAsync(user, dto.Profile!);
            if (profile == null)
            {
                return null;
            }

            option.Destination = profile.Destination;
        }
        else if (dto.Destination != null)
        {
            option.Destination = dto.Destination;
        }
        else
        {
            return null;
        }

        // Save the .zip archive to the server.
        string path = Path.GetTempFileName() + ".zip";
        using (FileStream stream = new(path, FileMode.Create))
        {
            dto.Archive.CopyTo(stream);
        }
        option.Source = path;

        return option;
    }
}