namespace BrandexSalesAdapter.Identity.Services.Identity
{
    using System.Threading.Tasks;
    using BrandexSalesAdapter.Services;
    using Data.Models;
    using Models.Identity;

    public interface IIdentityService
    {
        Task<Result<ApplicationUser>> Register(UserInputModel userInput);

        Task<Result<UserOutputModel>> Login(UserInputModel userInput);

        Task<Result> ChangePassword(string userId, ChangePasswordInputModel changePasswordInput);
    }
}
