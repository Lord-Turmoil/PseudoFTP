using PseudoFTP.Model.Database;
using PseudoFTP.Model.Dtos;

namespace PseudoFTP.Api.Services;

public interface IProfileService
{
    /// <summary>
    /// Get all profiles of a specific user.
    /// </summary>
    /// <param name="user"></param>
    /// <returns></returns>
    public Task<IEnumerable<ProfileDto>> GetProfilesAsync(User user);

    /// <summary>
    /// Get a profile of a specific user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="name"></param>
    /// <returns></returns>
    public Task<ProfileDto?> GetProfileAsync(User user, string name);

    /// <summary>
    /// Add a new profile to a specific user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="profileDto"></param>
    /// <returns></returns>
    public Task<ProfileDto?> AddProfileAsync(User user, AddProfileDto profileDto);

    /// <summary>
    /// Delete a profile of a specific user.
    /// </summary>
    /// <param name="user"></param>
    /// <param name="profileId"></param>
    /// <returns></returns>
    public Task DeleteProfileAsync(User user, int profileId);
}