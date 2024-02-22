using Arch.EntityFrameworkCore.UnitOfWork;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PseudoFTP.Model.Database;
using Tonisoft.AspExtensions.Module;

namespace PseudoFTP.Api.Services.Impl;

public class UserService : BaseService<UserService>, IUserService
{
    private readonly IRepository<User> _repo;

    public UserService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<UserService> logger, IRepository<User> repo)
        : base(unitOfWork, mapper, logger)
    {
        _repo = repo;
    }

    public async Task<User?> GetUserAsync(string username, string password)
    {
        User? user = await GetUserAsync(username);
        if (user == null)
        {
            return null;
        }

        return user.Password.Equals(password) ? user : null;
    }

    public Task<User?> GetUserAsync(string username)
    {
        return _repo.GetFirstOrDefaultAsync(
            predicate: u => u.Username.Equals(username),
            include: i => i.Include(u => u.Profiles)
        );
    }

    public ValueTask<User?> GetUserAsync(int userId)
    {
        return _repo.FindAsync(userId);
    }
}