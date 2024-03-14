using PseudoFTP.Model.Database;

namespace PseudoFTP.Api.Services;

public interface IUserService
{
    /// <summary>
    ///     Get a user with specific username and password. (With validation.)
    /// </summary>
    /// <param name="username"></param>
    /// <param name="password"></param>
    /// <returns></returns>
    public Task<User?> GetUserAsync(string username, string password);

    /// <summary>
    ///     Get a user with username only. No validation required.
    /// </summary>
    /// <param name="username"></param>
    /// <returns></returns>
    public Task<User?> GetUserAsync(string username);

    /// <summary>
    ///     Get a use via its ID.
    /// </summary>
    /// <param name="userId"></param>
    /// <returns></returns>
    public ValueTask<User?> GetUserAsync(int userId);
}