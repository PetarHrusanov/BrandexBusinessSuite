namespace BrandexSalesAdapter.Identity.Services.Identity;

using System.Linq;
using System.Threading.Tasks;
using BrandexSalesAdapter.Services;
using Data.Models;
using Microsoft.AspNetCore.Identity;
using Models.Identity;

public class IdentityService : IIdentityService
{
    private const string InvalidErrorMessage = "Invalid credentials.";

    private readonly UserManager<ApplicationUser> _userManager;
    private readonly ITokenGeneratorService _jwtTokenGenerator;

    public IdentityService(
        UserManager<ApplicationUser> userManager,
        ITokenGeneratorService jwtTokenGenerator)
    {
        _userManager = userManager;
        _jwtTokenGenerator = jwtTokenGenerator;
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