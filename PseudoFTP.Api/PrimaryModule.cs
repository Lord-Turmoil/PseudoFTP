using Arch.EntityFrameworkCore.UnitOfWork;
using PseudoFTP.Api.Services;
using PseudoFTP.Api.Services.Impl;
using PseudoFTP.Model.Database;
using Tonisoft.AspExtensions.Module;

namespace PseudoFTP.Api;

public class PrimaryModule : BaseModule
{
    public override IServiceCollection RegisterModule(IServiceCollection services)
    {
        services.AddCustomRepository<User, UserRepository>()
            .AddCustomRepository<TransferHistory, TransferHistoryRepository>()
            .AddCustomRepository<Profile, ProfileRepository>();

        services.AddScoped<IUserService, UserService>()
            .AddScoped<IProfileService, ProfileService>()
            .AddScoped<IStatusService, StatusService>()
            .AddScoped<ITransferService, TransferService>();

        return services;
    }
}