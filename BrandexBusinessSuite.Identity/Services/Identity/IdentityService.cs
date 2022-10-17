using AutoMapper;

namespace BrandexBusinessSuite.Identity.Services.Identity;

using System.Linq;
using System.Threading.Tasks;

using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

using BrandexBusinessSuite.Services;
using Data.Models;
using Models.Identity;

public class IdentityService : IIdentityService
{
    private const string InvalidErrorMessage = "Invalid credentials.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly RoleManager<ApplicationRole> _roleManager;
    private readonly ITokenGeneratorService _jwtTokenGenerator;
    private readonly IMapper _mapper;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        RoleManager<ApplicationRole> roleManager,
        ITokenGeneratorService jwtTokenGenerator,
        IMapper mapper
        )
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _jwtTokenGenerator = jwtTokenGenerator;
        _mapper = mapper;
    }

    public async Task<Result<ApplicationUser>> Register(UserInputModel userInput)
    {
        var user = new ApplicationUser
        {
            Email = userInput.Email,
            UserName = userInput.Email
        };

        var identityResult = await _userManager.CreateAsync(user, userInput.Password);

        var errors = identityResult.Errors.Select(e => e.Description);

        return identityResult.Succeeded
            ? Result<ApplicationUser>.SuccessWith(user)
            : Result<ApplicationUser>.Failure(errors);
    }

    public async Task RegisterWithRole(UserWithRoleInputModel userInput)
    {
        var user = new ApplicationUser
        {
            UserName = userInput.Email,
            Email = userInput.Email
        };

        await _userManager.CreateAsync(user, userInput.Password);
        await _userManager.AddToRoleAsync(user, userInput.Role);
    }

    public async Task<Result<UserOutputModel>> Login(UserInputModel userInput)
    {
        var user = await _userManager.FindByEmailAsync(userInput.Email);
        if (user == null) return InvalidErrorMessage;

        var passwordValid = await _userManager.CheckPasswordAsync(user, userInput.Password);
        if (!passwordValid) return InvalidErrorMessage;

        var roles = await _userManager.GetRolesAsync(user);

        var token = _jwtTokenGenerator.GenerateToken(user, roles);

        return new UserOutputModel(token);
    }

    public async Task CreateRole(string input)
    {
        if (!await _roleManager.RoleExistsAsync(input))
        { 
            await _roleManager.CreateAsync(new ApplicationRole(input));
        }
    }

    public async Task<string[]> GetRoles()
    {
        return await _roleManager.Roles.Select(r => r.Name).ToArrayAsync();
    }

    public async Task<UsersIds[]> GetUsers()
    {
        return await _mapper.ProjectTo<UsersIds>(_userManager.Users).ToArrayAsync();
    }

    public async Task<Result> ChangePassword(
        string userId,
        ChangePasswordInputModel changePasswordInput)
    {
        var user = await _userManager.FindByIdAsync(userId);
        if (user == null) return InvalidErrorMessage;

        var identityResult = await _userManager.ChangePasswordAsync(
            user,
            changePasswordInput.CurrentPassword,
            changePasswordInput.NewPassword);

        var errors = identityResult.Errors.Select(e => e.Description);

        return identityResult.Succeeded
            ? Result.Success
            : Result.Failure(errors);
    }
}