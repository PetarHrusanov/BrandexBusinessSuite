namespace BrandexSalesAdapter.ExcelLogic.Services.Identity
{
    using System;
    using System.Threading.Tasks;
    using BrandexSalesAdapter.ExcelLogic.Models.Identity;
    using Refit;


    public interface IIdentityService
    {
        [Post("/Identity/Login")]
        Task<UserOutputModel> Login([Body] UserInputModel loginInput);

        [Post("/Identity/Register")]
        Task<UserInputModel> Register([Body] UserInputModel loginInput);
    }
}
