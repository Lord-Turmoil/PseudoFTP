using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using PseudoFTP.Extension;
using PseudoFTP.Model.Data;
using Tonisoft.AspExtensions.Module;

namespace PseudoFTP.Api.Services.Impl;

public class StatusService : IStatusService
{
    //public StatusService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<StatusService> logger)
    //    : base(unitOfWork, mapper, logger)
    //{
    //}

    public StatusService()
    {
    }

    public StatusData GetStatus()
    {
        StatusData profile = new() {
            Profile = Globals.Profile,
            Version = Globals.Version
        };

        return profile;
    }
}