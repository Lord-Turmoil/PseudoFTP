using Microsoft.AspNetCore.Mvc;
using PseudoFTP.Api.Services;
using PseudoFTP.Model.Data;
using Tonisoft.AspExtensions.Module;
using Tonisoft.AspExtensions.Response;

namespace PseudoFTP.Api.Controllers;

[ApiController]
[Route("api/[controller]/[action]")]
public class StatusController : BaseController<StatusController>
{
    private readonly IStatusService _service;

    public StatusController(ILogger<StatusController> logger, IStatusService service)
        : base(logger)
    {
        _service = service;
    }

    [HttpGet]
    public ApiResponse GetStatus()
    {
        StatusData profile = _service.GetStatus();

        return new OkResponse(new OkDto(data: profile));
    }
}