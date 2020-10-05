//namespace BrandexSalesAdapter.ExcelLogic.Controllers
//{
//    using System;
//    using System.Threading.Tasks;
//    using BrandexSalesAdapter.ExcelLogic.Models.Identity;
//    using Microsoft.AspNetCore.Authorization;
//    using Microsoft.AspNetCore.Http;
//    using Microsoft.AspNetCore.Mvc;
//    using BrandexSalesAdapter.Infrastructure;
//    using BrandexSalesAdapter.ExcelLogic.Services.Identity;
//    using static BrandexSalesAdapter.Infrastructure.InfrastructureConstants;
//    using Microsoft.AspNetCore.Authentication;

//    public class IdentityController : HandleController
//    {

//        private readonly IIdentityService identityService;

//        public IdentityController(IIdentityService identityService)
//        {

//            this.identityService = identityService;
//        }

//        [HttpGet]
//        public async Task<IActionResult> Login()
//            => this.View();

//        [HttpPost]
//        public async Task<IActionResult> Login(UserInputModel model)
//        {
//            var result = await this.identityService
//                        .Login(model);

//            //await this.HttpContext.SignInAsync();

//            this.HttpContext.Response
//                 .Cookies.Append(
//                 AuthenticationCookieName,
//                 result.Token,
//                 new CookieOptions
//                 {
//                     HttpOnly = true,
//                     Secure = true,
//                     //Secure = true,
//                     MaxAge = TimeSpan.FromDays(1)
//                 });

//            return RedirectToAction(nameof(BrandexController.Index), "Brandex");
//        }

//        [HttpGet]
//        public async Task<IActionResult> Register()
//        {
//            return this.View();
//        }

//        [HttpPost]
//        public async Task<IActionResult> Register(UserInputModel user)
//        {
//            var input = await this.identityService.Register(user);

//            return await this.Login(input);

//        }

//        [AllowAnonymous]
//        public IActionResult Logout()
//        {
//            this.Response.Cookies.Delete(InfrastructureConstants.AuthenticationCookieName);

//            return RedirectToAction(nameof(HomeController.Index), "Home");
//        }

//    }
//}
