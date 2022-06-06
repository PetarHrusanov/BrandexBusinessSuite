namespace BrandexSalesAdapter.Identity.Controllers;

using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

using BrandexSalesAdapter.Controllers;
using BrandexSalesAdapter.Identity.Services.Identity;
using BrandexSalesAdapter.Services.Identity;
using BrandexSalesAdapter.Models;

using Models.Identity;
using Infrastructure;

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
    [AuthorizeAdministrator]
    public async Task Register(UserInputModel input)
    {
        await _identity.Register(input);

    }
    
    [HttpPost]
    [AuthorizeAdministrator]
    public async Task RegisterUserWithRole(UserWithRoleInputModel userWithRoleInputModel)
    {
        await _identity.RegisterWithRole(userWithRoleInputModel);
        
        // if (!result.Succeeded) return BadRequest(result.Errors);
        
    }
    
    [HttpPost]
    [AuthorizeAdministrator]
    public async Task CreateRole([FromBody]SingleStringInputModel singleStringInputModel)
    {
        await _identity.CreateRole(singleStringInputModel.SingleStringValue);

        // if (!result.Succeeded) return BadRequest(result.Errors);

    }
    
    [HttpGet]
    [AuthorizeAdministrator]
    public async Task<string[] > GetRoles()
    {
        return await _identity.GetRoles();

        // var roleStore = new RoleStore<IdentityRole>(new ApplicationUsersDbContext());
        // var roleManager = new RoleManager<IdentityRole>(roleStore);
        // if(!await roleManager.RoleExistsAsync("YourRoleName"))
        //     await roleManager.CreateAsync(new IdentityRole("YourRoleName"));


        // if (!result.Succeeded) return BadRequest(result.Errors);

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