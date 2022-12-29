namespace BrandexBusinessSuite.Services.Identity;

using System;
using System.Security.Claims;
using Infrastructure;
using Microsoft.AspNetCore.Http;

public class CurrentUserService : ICurrentUserService
{
    private readonly ClaimsPrincipal _user;

    public CurrentUserService(IHttpContextAccessor httpContextAccessor)
    {
        _user = httpContextAccessor.HttpContext?.User;

        if (_user == null)
        {
            throw new InvalidOperationException("This request does not have an authenticated user.");
        }

        UserId = _user.FindFirstValue(ClaimTypes.NameIdentifier);
    }

    public string UserId { get; }

    public bool IsAdministrator => _user.IsAdministrator();
}