﻿namespace BrandexBusinessSuite.Infrastructure;

using Microsoft.AspNetCore.Authorization;
using static Common.Constants;

public class AuthorizeAdministratorAttribute : AuthorizeAttribute
{
    public AuthorizeAdministratorAttribute() => Roles = AdministratorRoleName;
}