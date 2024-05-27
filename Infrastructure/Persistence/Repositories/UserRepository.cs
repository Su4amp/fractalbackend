using Application.Common.Interfaces.Entities.Users;
using Domain.Entities;
using Infrastructure.Persistence.DataContext;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories;

public class UserRepository : GenericRepository<User>, IUserRepository
{
    private readonly AppDbContext _dbContext;
    private readonly UserManager<User> _userManager;
    private readonly SignInManager<User> _signInManager;

    public UserRepository(
        AppDbContext dbContext,
        UserManager<User> userManager,
        SignInManager<User> signInManager) : base(dbContext)
    {
        _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
    }

    public async Task<SignInResult> CheckCredentials(User user, string password)
    {
        return await _signInManager.CheckPasswordSignInAsync(
            user: user,
            password: password,
            lockoutOnFailure: true);
    }

    public async Task<IdentityResult> RegisterUserAsync(User user)
    {
        return await _userManager.CreateAsync(user);
    }

    public async Task<IdentityResult> AddLoginAsync(User user, UserLoginInfo userLoginInfo)
    {
        return await _userManager.AddLoginAsync(user, userLoginInfo);
    }

    public async Task<IdentityResult> SetLockoutEnabledAsync(User user, bool enabled)
    {
        return await _userManager.SetLockoutEnabledAsync(user, enabled);
    }

    public async Task<IdentityResult> RegisterUserAsync(User user, string password)
    {
        return await _userManager.CreateAsync(user, password);
    }

    public async Task<User?> FindByLoginAsync(string loginProvider, string providerKey)
    {
        return await _userManager.FindByLoginAsync(loginProvider, providerKey);
    }

    public async Task<User?> FindByEmailAsync(string email)
    {
        return await _userManager.FindByEmailAsync(email);
    }

    public async Task<User?> GetUserByIdAsync(Guid userId)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Id == userId);
    }

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _dbContext.Users.SingleOrDefaultAsync(user => user.Email == email);
    }

    public async Task<User?> GetUserByUsernameAsync(string username)
    {
        return await _userManager.FindByNameAsync(username);
    }
}