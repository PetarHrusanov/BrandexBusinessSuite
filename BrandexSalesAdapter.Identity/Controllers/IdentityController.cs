namespace BrandexSalesAdapter.Identity.Controllers
{
    using BrandexSalesAdapter.Controllers;
    using BrandexSalesAdapter.Identity.Services.Identity;
    using BrandexSalesAdapter.Services.Identity;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.AspNetCore.Authorization;
    using Models.Identity;

    public class IdentityController : ApiController
    {
        private readonly IIdentityService _identity;
        private readonly ICurrentUserService _currentUser;

        public IdentityController(
            IIdentityService identity,
            ICurrentUserService currentUser)
        {
            _identity = identity;
            _currentUser = currentUser;
        }

        [HttpPost]
        public async Task<ActionResult<UserOutputModel>> Register(UserInputModel input)
        {
            var result = await _identity.Register(input);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return await Login(input);
        }

        [HttpPost]
        public async Task<ActionResult<UserOutputModel>> Login(UserInputModel input)
        {
            var result = await _identity.Login(input);

            if (!result.Succeeded) return BadRequest(result.Errors);

            return new UserOutputModel(result.Data.Token);
        }

        [HttpPut]
        [Authorize]
        [Route(nameof(ChangePassword))]
        public async Task<ActionResult> ChangePassword(ChangePasswordInputModel input)
        {
            return await _identity.ChangePassword(_currentUser.UserId, new ChangePasswordInputModel
            {
                CurrentPassword = input.CurrentPassword,
                NewPassword = input.NewPassword
            });
        }
    }
}