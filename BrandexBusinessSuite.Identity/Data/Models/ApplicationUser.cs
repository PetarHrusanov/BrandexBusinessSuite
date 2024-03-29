﻿namespace BrandexBusinessSuite.Identity.Data.Models;

using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;
using BrandexBusinessSuite.Data.Models.Common;

public class ApplicationUser : IdentityUser, IAuditInfo, IDeletableEntity
{
    public ApplicationUser()
    {
        Id = Guid.NewGuid().ToString();
        Roles = new HashSet<IdentityUserRole<string>>();
        Claims = new HashSet<IdentityUserClaim<string>>();
        Logins = new HashSet<IdentityUserLogin<string>>();
    }
    
    public DateTime CreatedOn { get; set; }
    public DateTime? ModifiedOn { get; set; }
    
    public bool IsDeleted { get; set; }

    public DateTime? DeletedOn { get; set; }

    public virtual ICollection<IdentityUserRole<string>> Roles { get; set; }
    public virtual ICollection<IdentityUserClaim<string>> Claims { get; set; }
    public virtual ICollection<IdentityUserLogin<string>> Logins { get; set; }

}