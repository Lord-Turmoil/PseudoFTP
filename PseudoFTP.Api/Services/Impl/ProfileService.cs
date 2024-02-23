using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;
using Tonisoft.AspExtensions.Module;
using Profile = PseudoFTP.Model.Database.Profile;

namespace PseudoFTP.Api.Services.Impl;

public class ProfileService : BaseService<ProfileService>, IProfileService
{
    private readonly IRepository<Profile> _repo;

    public ProfileService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<ProfileService> logger,
        IRepository<Profile> repo)
        : base(unitOfWork,
            mapper, logger)
    {
        _repo = repo;
    }

    public async Task<IEnumerable<ProfileDto>> GetProfilesAsync(User user)
    {
        IList<Profile> profiles;
        if (user.Profiles != null)
        {
            profiles = user.Profiles.ToList();
        }
        else
        {
            // In case the user is not loaded with profiles.
            profiles = await _repo.GetAllAsync(predicate: p => p.UserId == user.Id);
        }

        return profiles.Select(_mapper.Map<Profile, ProfileDto>);
    }

    public async Task<ProfileDto?> GetProfileAsync(User user, string name)
    {
        Profile? profile = await _repo.GetFirstOrDefaultAsync(
            predicate: p => p.UserId == user.Id && p.Name.Equals(name));
        return profile == null ? null : _mapper.Map<Profile, ProfileDto>(profile);
    }

    public async Task<ProfileDto?> AddProfileAsync(User user, AddProfileDto profileDto)
    {
        if (await _repo.ExistsAsync(p => p.UserId == user.Id && p.Name.Equals(profileDto.Name)))
        {
            return null;
        }

        Profile profile = _mapper.Map<AddProfileDto, Profile>(profileDto);
        profile.UserId = user.Id;
        EntityEntry<Profile> entity = await _repo.InsertAsync(profile);
        await _unitOfWork.SaveChangesAsync();

        return _mapper.Map<Profile, ProfileDto>(entity.Entity);
    }

    public async Task DeleteProfileAsync(User user, string name)
    {
        Profile? profile = await _repo.GetFirstOrDefaultAsync(
            predicate: p => p.Name.Equals(name) && p.UserId == user.Id);
        if (profile != null)
        {
            _repo.Delete(profile.Id);
            await _unitOfWork.SaveChangesAsync();
        }
    }
}